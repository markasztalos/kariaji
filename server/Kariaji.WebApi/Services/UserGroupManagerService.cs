using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Middlewares;
using Kariaji.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public User GetUserById(int id)
        {
            return this.ctx.Users.FirstOrDefault(u => u.Id == id);
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
    }

}
