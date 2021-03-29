import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;
  genderList = [{value: 'male', display: 'Nam'}, {value: 'female', display: 'Nữ'}];
  constructor(private menberService: MembersService, private accountService: AccountService) { 
    accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.menberService.getMembers(this.userParams).subscribe(response => {
        this.members = response.result;
        this.pagination = response.pagination;
        this.pagination.currentPage = 1;
    })
  }
  pageChange(event) {
    this.userParams.pageNumber = event.page;
    this.loadMembers();
  }

  resetFilter() {
    this.userParams = new UserParams(this.user);
    this.loadMembers();
  }
}
