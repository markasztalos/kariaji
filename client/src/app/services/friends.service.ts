import { Injectable } from "@angular/core";
import { IKariajiAppState } from "../store/app.state";
import { NgRedux } from "@angular-redux/store";
import { ContainerGroupsStateService } from "../store/container-groups.redux";
import { AvatarsStateService, AvatarsActions } from "../store/avatars.redux";
import { UsersStateService } from "../store/user-groups.redux";
import { UserGroupApiService } from "./user-group-adi.service";
import { NgStoreService } from "../store/app.store";

@Injectable()
export class FriendsService {

    constructor(private ngRedux: NgRedux<IKariajiAppState>,
        private cgStateSvc: ContainerGroupsStateService,
        private ugApiSvc: UserGroupApiService,
        private storeActions: NgStoreService,
        private avatarActions: AvatarsActions,
        private usersStateSvc: UsersStateService) {

    }

    private isQueryPending: boolean = false;
    public ensureFriends() {
        if (this.ngRedux.getState().friendsQueried)
            return;
        if (this.isQueryPending)
            return;
        this.isQueryPending = true;

        this.ugApiSvc.getDataOfFriends().subscribe(result => {
            this.storeActions.setFriendsQueried(true);
            this.isQueryPending = false;
            this.cgStateSvc.setContainerGroups(result.friendGroups);
            this.usersStateSvc.setUsersInRedux(result.friendUsers);

            // for (const userId of result.friendUsers.map(u => u.id)) {
            //     const avatar = result.friendAvatars.find(a => a.userId === userId);
            //     if (avatar) {
            //         console.log(avatar.data);
            //         const blob = new Blob(avatar.data, { type: avatar.contentType });
            //         const localUlr = URL.createObjectURL(blob);
            //         this.avatarActions.setAvatarUrl(userId, localUlr);
            //     } else {
            //         this.avatarActions.setAvatarUrl(userId, null);
            //     }
            // }
        });
    }
}