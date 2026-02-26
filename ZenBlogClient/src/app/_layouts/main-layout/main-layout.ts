import { AfterViewInit, Component, OnInit } from '@angular/core';
import { AuthService } from '../../_services/auth-service';
import { SocialService } from '../../_services/social-service';
import { SocialDto } from '../../_models/socialDto';
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

  constructor(
    private authService: AuthService,
    private socialService: SocialService
  ){}

  private swiper: Swiper | undefined;
  isMobileMenuOpen = false;

  ngOnInit() {
    // Load social icons/links from backend (Admin panel controls these)
    this.socialService.getAll().subscribe({
      next: (items: any) => {
        this.socials = Array.isArray(items) ? items : [];
      },
      error: () => {
        // no-op (fallback to static icons in template)
        this.socials = [];
      }
    });

    // Initialize AOS
    AOS.init({
      duration: 1000,
      easing: 'ease-in-out',
      once: true,
      mirror: false
    });

    // Initialize Swiper
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

    // Handle scroll top button
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
    // Remove preloader after view is initialized
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
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }


  getFullName(){
    let decodedToken =  this.authService.decodeToken();

    return decodedToken.fullName;
  }


  loggedIn(){
    return this.authService.loggedIn();
  }

  isIconClass(icon: any): boolean {
    if (typeof icon !== 'string' || !icon) return false;
    // Heuristic: old UI stored CSS class names. New UI stores image URLs (/uploads/... or http(s)://...)
    const looksLikePath = icon.includes('/') || icon.includes('\\') || /^https?:\/\//i.test(icon) || /\.(png|jpe?g|webp|svg)$/i.test(icon);
    return !looksLikePath;
  }

  iconSrc(icon: any): string {
    if (typeof icon !== 'string') return '';
    return icon;
  }

}
