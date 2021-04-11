import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { MessageParams } from '../_models/messageParams';
import { getPaginationHeaders, getPaginationResult } from './paginationHelpers';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  messageParams: MessageParams;
  constructor(private http: HttpClient) { }

  getMessages(messageParams) {
    let params = getPaginationHeaders(messageParams.pageNumber, messageParams.pageSize);
    params = params.append('Container', messageParams.container);
    return getPaginationResult<Message[]>(this.baseUrl + 'messages', params,this.http);
  }
}
