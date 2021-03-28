import { User } from "./user";

export class UserParams {
    gender: string;
    minAge = 18;
    maxAge = 99;
    pageSize = 1;
    pageNumber = 1;
    orderBy = "lastActive";

    constructor(user: User) {
        this.gender = user.gender === 'male' ? 'female' : 'male';
    }
}