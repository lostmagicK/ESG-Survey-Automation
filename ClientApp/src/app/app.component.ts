import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Configuration } from '../../build';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  auth = false;
  userName = '';
  constructor(private router: Router, private configuration: Configuration){
    this.router.events.subscribe(data => {
      if (data instanceof NavigationEnd){
        var token = localStorage.getItem('accessToken');
        if(token && token.length > 1 && !this.auth){
          this.auth = true;
          this.configuration.credentials['bearer'] = `bearer ${token}`;
          if(['/sign-in','/sign-up'].indexOf(data.url) > -1){
            this.router.navigate(['/dashboard']);
          }
          this.userName = localStorage.getItem('userName') ?? '';
        } else if (this.auth){
          this.auth = false;
          this.userName = '';
        }
      }
    });
  }
  signOut(){
    localStorage.clear();
    this.auth = false;
    this.userName = '';
    this.configuration.credentials['bearer'] = '';
    this.router.navigate(['/sign-in']);
  }
}
