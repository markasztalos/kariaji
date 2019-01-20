using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Middlewares;
using Kariaji.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Kariaji.WebApi.Services
{
    public class UserGroupManagerService
    {
        private readonly KariajiContext ctx;
        private readonly MailingService mailingSvc;
        private readonly ProtectionService protectionService;
        private readonly ConfigurationProviderService configSvc;

        public UserGroupManagerService(KariajiContext ctx, ProtectionService protectionService,
            MailingService mailingSvc, ConfigurationProviderService configSvc)
        {
            this.ctx = ctx;
            this.protectionService = protectionService;
            this.mailingSvc = mailingSvc;
            this.configSvc = configSvc;
        }


        public async Task<User> UpdateUserAccount(int userId, string displayName)
        {
            var user = this.ctx.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw KariajiException.NewPublic("A felhasználó nem létezik");

            user.DisplayName = displayName;
            await this.ctx.SaveChangesAsync();
            return user;
        }

        public async Task UpdatePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = this.ctx.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw KariajiException.NewPublic("A felhasználó nem létezik");
            if (user.Password != this.protectionService.HashPassword(oldPassword))
                throw KariajiException.NewPublic("Hibás jelszó");
            user.Password = this.protectionService.HashPassword(newPassword);
            await this.ctx.SaveChangesAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await this.ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<List<User>> GetUsersByIds(IReadOnlyList<int> ids)
        {
            return await this.ctx.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Membership>> GetMemberships(int currentUserId)
        {
            return await ctx.Memberships.Include(m => m.Group).Where(m => m.UserId == currentUserId && !m.IsDeleted)
                .ToListAsync();
        }

        public async Task<ActionResult<List<string>>> GetExistingGroupNames()
        {
            return await this.ctx.Groups.Select(g => g.DisplayName).ToListAsync();
        }

        public async Task<bool> IsManagedUser(int userId)
        {
            return (await ctx.Users.Where(u => u.Id == userId).Select(u => u.IsManaged).FirstOrDefaultAsync());
        }
        public async Task<Group> CreateNewGroupAsync(CreateNewGroupModel model, int currentUserId)
        {
            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (string.IsNullOrEmpty(model.Name))
                throw KariajiException.NewPublic("A név nem lehet üres");

            var existingGroup =
                await this.ctx.Groups.FirstOrDefaultAsync(g => g.DisplayName.ToLower() == model.Name.ToLower());
            if (existingGroup != null)
                throw KariajiException.NewPublic("Ilyen nevű csoport már létezik");

            var group = new Group
            {
                CreatorUserId = currentUserId,
                CreationDate = DateTime.Now,
                Description = model.Description,
                DisplayName = model.Name,
                Memberships = new List<Membership>()
            };
            group.Memberships.Add(new Membership
            {
                UserId = currentUserId,
                Group = group,
                IsAdministrator = true,
            });
            ctx.Groups.Add(group);
            await ctx.SaveChangesAsync();
            return group;

        }

        public async Task<Group> GetGroupByIdAsync(int groupId)
        {
            return await this.ctx.Groups.Include(g => g.Memberships).ThenInclude(m => m.User).FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<ActionResult<List<string>>> GetExistingEmailAddressesAsync(int[] exceptUserIds)
        {
            return await this.ctx.Users.Where(u => !u.IsDeleted && !exceptUserIds.Contains(u.Id)).Select(u => u.Email).ToListAsync();

        }

        public async Task InviteAsync(int groupId, string email, int senderUserId)
        {
            email = email.Trim();
            var existingGroup = await this.ctx.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (existingGroup == null)
                throw KariajiException.NewPublic("Ilyen csoport nem létezik");
            var existingMembership = await this.ctx.Memberships.FirstOrDefaultAsync(m => !m.IsDeleted &&
                m.GroupId == groupId && m.User.Email.ToLower() == email.ToLower());
            if (existingMembership != null)
                throw KariajiException.NewPublic("Ez a felhasználó már tagja a csoportnak");
            var existingInvitation = await ctx.Invitations.FirstOrDefaultAsync(i =>
                i.GroupId == groupId && i.InvitedEmail.ToLower() == email.ToLower());
            if (existingInvitation != null)
                throw KariajiException.NewPublic("Erre az email címre már küldtünk meghívót");
            var existingUser = await ctx.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (existingUser == null)
                throw KariajiException.NewPublic("Nincsen ilyen email címmel regisztrált felhasználó");
            ctx.Invitations.Add(new UserGroupInvitation
            {
                GroupId = groupId,
                InvitedEmail = email,
                InvitedUserId = existingUser.Id,
                SenderUserId = senderUserId,
                SendingDate = DateTime.Now
            });
            await ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserGroupInvitation>> GetInvitationsOfGroupAsync(int groupId, int userId)
        {
            if (!(await this.CanAdministerGroup(groupId, userId)))
                throw KariajiException.NewPublic("Nincs jogosultsága ehhez a művelethez");
            return await this.ctx.Invitations.Include(i => i.Group).Include(i => i.SenderUser).Include(i => i.InvitedUser).Where(i => i.GroupId == groupId).ToListAsync();
        }

        public async Task<IEnumerable<UserGroupInvitation>> GetMyInvitations(int currentUserId)
        {
            return await this.ctx.Invitations.Include(i => i.Group).Include(i => i.SenderUser).Include(i => i.InvitedUser).Where(i => i.InvitedUser.Id == currentUserId).ToListAsync();
        }

        public async Task<bool> IsMemberOfGroup(int groupId, int userId)
        {
            return await ctx.Groups.AnyAsync(g =>
                g.Id == groupId && g.Memberships.Any(m => m.UserId == userId && !m.IsDeleted));
        }
        public async Task<bool> IsMemberOfGroups(IReadOnlyCollection<int> groupIds, int userId)
        {
            var gids = await ctx.Groups.Where(g =>
                groupIds.Contains(g.Id) && g.Memberships.Any(m => m.UserId == userId && !m.IsDeleted)).Select(g=>g.Id).ToListAsync();
            return gids.Count == groupIds.Count;
        }


        public async Task<bool> CanAdministerGroup(int groupId, int userId)
        {
            var adminMembership = await this.ctx.Memberships.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && !m.IsDeleted && !m.Group.IsDeleted && !m.User.IsDeleted && m.IsAdministrator);
            return adminMembership != null;
        }

        public async Task DeleteInvitationAsync(int invitationId, int currentUserId)
        {
            var inv = await this.ctx.Invitations.FirstOrDefaultAsync(i => i.Id == invitationId);
            if (inv == null)
                return;

            if (!(await this.CanAdministerGroup(inv.GroupId, currentUserId)))
                throw KariajiException.NewPublic("Nincs jogosultságod ehhez a művelethez");

            this.ctx.Invitations.Remove(inv);
            await this.ctx.SaveChangesAsync();
        }

        public async Task AcceptInvitationAsync(int invitationId, int currentUserId)
        {
            var inv = await this.ctx.Invitations.FirstOrDefaultAsync(i => i.Id == invitationId);
            if (inv == null)
                return;

            if (inv.InvitedUser.Id != currentUserId)
                throw KariajiException.NewPublic("Nincs jogosultságod ehhez a művelethez");

            this.ctx.Invitations.Remove(inv);

            var deletedMembership = await ctx.Memberships.FirstOrDefaultAsync(m =>
                m.UserId == currentUserId && m.GroupId == inv.GroupId && m.IsDeleted);
            if (deletedMembership != null)
                deletedMembership.IsDeleted = false;
            else
                this.ctx.Memberships.Add(new Membership
                {
                    UserId = currentUserId,
                    GroupId = inv.GroupId,
                    IsAdministrator = false,
                    IsDeleted = false
                });
            await this.ctx.SaveChangesAsync();
        }

        public async Task RejectInvitationAsync(int invitationId, int currentUserId)
        {
            var inv = await this.ctx.Invitations.FirstOrDefaultAsync(i => i.Id == invitationId);
            if (inv == null)
                return;

            if (inv.InvitedUser.Id != currentUserId)
                throw KariajiException.NewPublic("Nincs jogosultságod ehhez a művelethez");

            this.ctx.Invitations.Remove(inv);
            await this.ctx.SaveChangesAsync();
        }

        public async Task<Avatar> GetAvatarOfUserAsync(int userId)
        {
            return await this.ctx.Avatars.FirstOrDefaultAsync(a => a.UserId == userId);
        }
        public async Task<List<Avatar>> GetAvatarsOfUsersAsync(IReadOnlyList<int> userIds)
        {
            return await this.ctx.Avatars.Where(a => userIds.Contains(a.UserId)).ToListAsync();
        }

        public async Task UpdateAvatarAsync(Avatar avatar)
        {
            var existingAvatar = await this.ctx.Avatars.FirstOrDefaultAsync(a => a.UserId == avatar.UserId);
            if (existingAvatar == null)
            {
                this.ctx.Avatars.Add(avatar);
            }
            else
            {
                existingAvatar.Data = avatar.Data;
                existingAvatar.ContentType = avatar.ContentType;
            }

            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAvatar(int userId)
        {
            var existingAvatar = await this.ctx.Avatars.FirstOrDefaultAsync(a => a.UserId == userId);
            if (existingAvatar != null)
            {
                this.ctx.Avatars.Remove(existingAvatar);
            }

            await this.ctx.SaveChangesAsync();
        }

        public async Task<bool> AreUsersFriends(int u1, int u2)
        {
            return await this.ctx.Groups.AnyAsync(g =>
                g.Memberships.Any(m => !m.IsDeleted && m.UserId == u1) &&
                g.Memberships.Any(m => !m.IsDeleted && m.UserId == u2));
        }

        public async Task<Membership> UpdateMembership(int groupId, int userId, bool isAdmin)
        {
            var membership = await ctx.Memberships.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && !m.IsDeleted);
            if (membership == null)
                throw KariajiException.BadParamters;
            membership.IsAdministrator = isAdmin;
            await ctx.SaveChangesAsync();
            return membership;
        }

        public async Task DeleteMembership(int groupId, int userId)
        {
            var membership = await ctx.Memberships.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && !m.IsDeleted);
            if (membership == null)
                throw KariajiException.BadParamters;
            membership.IsDeleted = true;
            await ctx.SaveChangesAsync();
        }


        public async Task<List<Group>> GetContainerGroupsAsync(int currentUserId)
        {
            return await this.ctx.Groups
                .Include(g => g.Memberships)
                .ThenInclude(m => m.User)
                .Where(g => g.Memberships.Any(m => m.UserId == currentUserId && !m.IsDeleted))
                .ToListAsync();
        }

        public async Task<List<int>> GetContainerGroupIdsAsync(int currentUserId)
        {
            return await this.ctx.Groups
                .Include(g => g.Memberships)
                .ThenInclude(m => m.User)
                .Where(g => g.Memberships.Any(m => m.UserId == currentUserId && !m.IsDeleted))
                .Select(g => g.Id)
                .ToListAsync();
        }

        public async Task<List<int>> GetFriendUserIds(int currentUserId)
        {
            var userIds = await
                ctx.Groups.Where(g => !g.IsDeleted && g.Memberships.Any(m => !m.IsDeleted && m.UserId == currentUserId))
                    .SelectMany(g => g.Memberships.Where(m => !m.IsDeleted).Select(m => m.UserId)).ToListAsync();
            return userIds;
        }

        public async Task<bool> IsManagedUserNameFree(int managerUserId, string displayName)
        {
            var displayNameToLower = displayName.ToLower().Trim();
            return !await ctx.Users.AnyAsync(u =>
                u.IsManaged && u.ManagerUsers.Any(m => m.ManagerUserId == managerUserId) &&
                u.DisplayName.ToLower() == displayName);
        }

        public async Task<User> CreateManagedUserAsync(int managerUserId, string displayName)
        {
            
            var user = new User
            {
                DisplayName = displayName,
                IsManaged = true,
            };
            var management = new UserManagement
            {
                ManagedUser = user,
                ManagerUserId = managerUserId
            };
            user.ManagerUsers.Add(management);
            this.ctx.Users.Add(user);
            await this.ctx.SaveChangesAsync();
            return user;
        }


        public async Task<bool> IsManagerOfUser(int managerUserId, int managedUserId)
        {
            return await this.ctx.UserManagements.AnyAsync(m =>
                m.ManagedUserId == managedUserId && m.ManagerUserId == managerUserId && !m.ManagerUser.IsDeleted);
        }



        public async Task DeleteManagedUser(int managedUserId)
        {
            var user = await this.ctx.Users.FirstOrDefaultAsync(u => u.Id == managedUserId);
            user.IsDeleted = true;
            //var managements = await this.ctx.UserManagements.Where(m => m.ManagedUserId == managedUserId).ToListAsync();
            //ctx.UserManagements.RemoveRange(managements);
            await ctx.SaveChangesAsync();
        }



        public async Task AddManagerToUser(int managedUserId, int managerUserId)
        {
            var management = new UserManagement
            {
                ManagedUserId = managedUserId,
                ManagerUserId = managerUserId
            };
            ctx.UserManagements.Add(management);
            await ctx.SaveChangesAsync();
        }

        
        public async Task<bool> CanRemoveManagerOfUser(int currentUserId, int managerUserId, int managedUserId)
        {
            return await this.IsManagerOfUser(currentUserId, managedUserId) &&
                   (managerUserId == currentUserId ? 
                    ((await this.ctx.UserManagements.Where(m => m.ManagedUserId ==managedUserId).CountAsync()) > 1)
                   : (await this.IsManagerOfUser(managerUserId, managedUserId))
                   );
        }

        public async Task RemoveManagerOfUser(int managerUserId, int managedUserId)
        {
            var management = await ctx.UserManagements.FirstOrDefaultAsync(m =>
                m.ManagedUserId == managerUserId && m.ManagedUserId == managedUserId);
            ctx.UserManagements.Remove(management);
            await ctx.SaveChangesAsync();
        }

        public async Task<Membership> AddManagedUserToGroup(int managedUserId, int groupId)
        {
            var m = new Membership
            {
                UserId = managedUserId,
                GroupId = groupId,
                IsAdministrator = false,
                IsDeleted = false
            };
            this.ctx.Memberships.Add(m);
            await this.ctx.SaveChangesAsync();
            return m;
        }


        
    }

}
