import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model : any = {}

  currentUser$: Observable<User>;

  constructor(private accountService : AccountService, private router: Router,
    private toastr: ToastrService) { }

  ngOnInit(): void {
    this.currentUser$ = this.accountService.currentUser$;
  }

  login() {
    this.accountService.login(this.model).subscribe(response=>{
      this.toastr.success("Đăng nhập thành công")
      this.router.navigateByUrl('/members');
    }, error => {
      this.toastr.error(error.error)
      console.log(error);
    })
  }

  logout() {
    this.router.navigateByUrl('/');
    this.accountService.logout();
  }

}
