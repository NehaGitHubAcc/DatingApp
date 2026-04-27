import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal} from '@angular/core';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App implements OnInit {
  //Component Lifecycle hook
  //Step 1: Constructor Inject HttpClient using the inject function
  private http = inject(HttpClient);
  protected title = 'Dating App';
  protected members = signal<any>([]);


  //Step 2: Implement the ngOnInit lifecycle hook to make an HTTP GET request to the API endpoint
  async ngOnInit() { 
    this.members.set(await this.getMembers());
  }

  async getMembers() {
    try {
      return lastValueFrom(this.http.get('https://localhost:5001/api/members'));
    } 
    catch (error) {
      console.log(error);
      throw error; // Rethrow the error after logging it
    }
  }

}
