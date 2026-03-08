import { AfterViewInit, Component, OnInit } from '@angular/core';
import { AuthService } from '../../_services/auth-service';
import { SocialService } from '../../_services/social-service';
import { SocialDto } from '../../_models/socialDto';
import { UserDto } from '../../_models/userDto';
import { Autoplay, Navigation, Pagination } from 'swiper/modules';
import Swiper from 'swiper';
import AOS from 'aos';

@Component({
  selector: 'main-layout',
  standalone: false,
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayout implements OnInit, AfterViewInit {
  socials: SocialDto[] = [];
  currentUser: UserDto | null = null;
  isAdminUser = false;

  constructor(
    private authService: AuthService,
    private socialService: SocialService
  ){}

  private swiper: Swiper | undefined;
  isMobileMenuOpen = false;

  ngOnInit() {
    this.socialService.getAll().subscribe({
      next: (items: any) => {
        this.socials = Array.isArray(items) ? items : [];
      },
      error: () => {
        this.socials = [];
      }
    });

    if (this.loggedIn()) {
      this.authService.getCurrentUser(true).subscribe(user => this.currentUser = user);
      this.authService.currentUser$.subscribe(user => this.currentUser = user);
      this.authService.refreshAdminStatus().subscribe(isAdmin => this.isAdminUser = isAdmin);
      this.authService.isAdmin$.subscribe(isAdmin => this.isAdminUser = isAdmin);
    }

    AOS.init({
      duration: 1000,
      easing: 'ease-in-out',
      once: true,
      mirror: false
    });

    const swiperContainer = document.querySelector('.init-swiper');
    if (swiperContainer) {
      try {
        this.swiper = new Swiper('.init-swiper', {
          modules: [Navigation, Pagination, Autoplay],
          loop: true,
          speed: 600,
          autoplay: {
            delay: 5000,
            disableOnInteraction: false
          },
          slidesPerView: 'auto',
          centeredSlides: true,
          pagination: {
            el: '.swiper-pagination',
            type: 'bullets',
            clickable: true
          },
          navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev'
          }
        });
      } catch {
        this.swiper = undefined;
      }
    }

    window.addEventListener('scroll', () => {
      const scrollTop = document.querySelector('.scroll-top');
      if (scrollTop) {
        if (window.scrollY > 100) {
          scrollTop.classList.add('active');
        } else {
          scrollTop.classList.remove('active');
        }
      }
    });
  }

  ngAfterViewInit() {
    const preloader = document.querySelector('#preloader');
    if (preloader) {
      preloader.remove();
    }
  }

  toggleMobileMenu() {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
    const navmenu = document.querySelector('#navmenu');
    if (navmenu) {
      if (this.isMobileMenuOpen) {
        navmenu.classList.add('mobile-nav-active');
      } else {
        navmenu.classList.remove('mobile-nav-active');
      }
    }
  }

  scrollToTop(event: Event) {
    event.preventDefault();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  loggedIn(){
    return this.authService.loggedIn();
  }

  logout() {
    this.authService.logout();
  }

  get displayName() {
    return this.currentUser?.fullName || this.authService.getFullName() || this.authService.getUserName() || 'User';
  }

  get userImage() {
    return this.currentUser?.imageUrl || null;
  }

  get userEmail() {
    return this.currentUser?.email || this.getUserName();
  }

  getUserName() {
    return this.authService.getUserName();
  }

  get initials() {
    return this.displayName
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(x => x[0])
      .join('')
      .toUpperCase();
  }

  isIconClass(icon: any): boolean {
    if (typeof icon !== 'string' || !icon) return false;
    const looksLikePath = icon.includes('/') || icon.includes('\\') || /^https?:\/\//i.test(icon) || /\.(png|jpe?g|webp|svg)$/i.test(icon);
    return !looksLikePath;
  }

  iconSrc(icon: any): string {
    if (typeof icon !== 'string') return '';
    return icon;
  }
}
