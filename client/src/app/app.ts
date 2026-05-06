import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { Nav } from "../layout/nav/nav";
import { AccountService } from '../core/services/account-service';
import { User } from '../types/user';
import { Home } from "../features/home/home";

@Component({
  selector: 'app-root',
  imports: [Nav, Home],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App implements OnInit {
  //Component Lifecycle hook
  //Step 1: Constructor Inject HttpClient using the inject function
  private accountService = inject(AccountService);
  private http = inject(HttpClient);
  protected title = 'Dating app';
  protected members = signal<User[]>([]);


  //Step 2: Implement the ngOnInit lifecycle hook to make an HTTP GET request to the API endpoint
  async ngOnInit() { 
    this.members.set(await this.getMembers());
    this.setCurrentUser();
  }

  //Step 3: Create a method to set the current user from local storage
  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }

  //Step 4: Create a method to fetch members from the API and handle errors
  async getMembers() {
    try {
      return lastValueFrom(this.http.get<User[]>('https://localhost:5001/api/members'));
    } 
    catch (error) {
      console.log(error);
      throw error; // Rethrow the error after logging it
    }
  }

}
