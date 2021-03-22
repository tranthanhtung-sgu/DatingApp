import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() usersFormHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];

  constructor(private accountService: AccountService, private toastr: ToastrService,
    private fb: FormBuilder, private route: Router) { }

  ngOnInit(): void {
    this.initializeFrom();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);
  }

  initializeFrom() {
    this.registerForm = this.fb.group({
      gender: ['',Validators.required],
      username: ['',Validators.required],
      knownAs: ['',Validators.required],
      dateOfBirth: ['',Validators.required],
      city: ['',Validators.required],
      country: ['',Validators.required],
      password: ['',[Validators.required, Validators.minLength(8)]],
      confirmPassword: ['',[Validators.required, this.matchValue('password')]]
    })
  }

  register() { 
    this.accountService.register(this.registerForm.value).subscribe(response => {
      console.log(response);
      this.route.navigateByUrl('/members');
    }, error => {
      this.validationErrors = error;
      console.log(error);
    })
  }

  matchValue(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
        ? null : {isMatch: true}
    }
  }

  cancel() {
    this.cancelRegister.emit(false);  
  }

}
