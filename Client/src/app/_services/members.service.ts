import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { getPaginationHeaders, getPaginationResult } from './paginationHelpers';


@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();

  constructor(private http: HttpClient) { }

  getMembers(userParams: UserParams) {
    var response = this.memberCache.get(Object.values(userParams).join('-'));  
    if (response) {
      return of(response);
    }    

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    
    return getPaginationResult<Member[]>(this.baseUrl + 'users', params, this.http)
      .pipe(map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      }))
  }

  

  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.username === username);
      console.log(member);
      
      if (member) {
        return of(member);
      }
    
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }
  
  addLike(username: string){
    console.log(username)
    return this.http.post(this.baseUrl+ "likes/"+username, {});
  }

  getUsersLike(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber,pageSize);
    params = params.append('predicate', predicate);
    return getPaginationResult<Partial<Member[]>>(this.baseUrl+ 'likes/', params, this.http);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users/', member).pipe(
      map(()=>{
        const index = this.members.indexOf(member);
        this.members[index] =  member;
      })
    )
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl+ 'users/delete-photo/' + photoId);
  }
}
