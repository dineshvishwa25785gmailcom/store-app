import { Component, DoCheck, OnInit, effect, signal, ViewChild } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { MaterialModule } from '../../material.module';
import { UserService } from '../../_service/user.service';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { Menu } from '../../_model/user.model';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../_service/authentication.service';
import { MatDrawer } from '@angular/material/sidenav';

@Component({
  selector: 'app-appmenu',
  standalone: true,
  imports: [CommonModule, MaterialModule, RouterOutlet, RouterLink],
  templateUrl: './appmenu.component.html',
  styleUrl: './appmenu.component.css',
})
export class AppmenuComponent implements OnInit, DoCheck {
  @ViewChild('drawer') drawer!: MatDrawer;
  menulist = signal<Menu[]>([]);
  Loginuser = '';
  showmenu = false;
  isSuperAdmin = false;

  isHandset$: Observable<boolean>;

  constructor(
    private breakpointObserver: BreakpointObserver,
    private service: UserService,
    private router: Router,
    private authService: AuthService
  ) {
    this.isHandset$ = this.breakpointObserver.observe(Breakpoints.Handset)
      .pipe(
        map(result => result.matches),
        shareReplay()
      );
    // React to authentication state changes
    // When user logs in or out, reload the menu
    effect(() => {
      const isAuth = this.authService.isAuthenticated$();
      if (isAuth) {
        this.loadMenuItems();
      } else {
        this.menulist.set([]);
      }
    });
  }

  ngOnInit(): void {
    // Initial menu load if already authenticated
    if (this.authService.getAuthStatus()) {
      this.loadMenuItems();
    }
  }

  /**
   * Load menu items based on user role from localStorage
   */
  private loadMenuItems(): void {
    const userrole = this.authService.getUserRole();
    if (userrole) {
      this.service.loadMenuByRole(userrole).subscribe({
        next: (item) => {
          console.log('AppmenuComponent: Menu items loaded successfully:', item);
          this.menulist.set(item);
        },
        error: (error) => {
          console.error('AppmenuComponent: Failed to load menu items:', error);
          this.menulist.set([]);
        }
      });
    }
  }

  ngDoCheck(): void {
    this.Loginuser = this.authService.getUsername() || '';
    let userrole = this.authService.getUserRole() || '';
    this.isSuperAdmin = userrole.toLowerCase() === 'super_admin' || userrole.toLowerCase() === 'superadmin';
    this.Setaccess();
  }

  Setaccess() {
    let currentUrl = this.router.url;
    if (
      currentUrl === '/register' ||
      currentUrl === '/login' ||
      currentUrl === '/resetpassword' ||
      currentUrl === '/forgetpassword'
    ) {
      this.showmenu = false;
    } else {
      // Only show menu when user is authenticated
      this.showmenu = this.authService.getAuthStatus();
    }
  }

  toggleDrawer() {
    this.drawer.toggle();
  }

  closeDrawerOnItemClick() {
    if (this.drawer.mode === 'over') {
      this.drawer.close();
    }
  }
}
