import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { MessageParams } from '../_models/messageParams';
import { Pagination } from '../_models/pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messageParams: MessageParams;
  messages: Message[];
  pagination: Pagination;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.messageService.getMessages(this.messageParams).subscribe(response => {
      this.messages = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChaned(event: any) {
    this.pagination = event.page;
    this.loadMessages();
  }

}
