import { AfterViewInit, Component, OnInit } from '@angular/core';
import Swiper from 'swiper';
import { Autoplay, Navigation, Pagination } from 'swiper/modules';
import AOS from 'aos';
import { BlogService } from '../../_services/blog-service';
import { BlogDto } from '../../_models/blog';
import { CategoryService } from '../../_services/category-service';
import { CategoryDto } from '../../_models/category';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'home',
  standalone: false,
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit, AfterViewInit  {
  swiper: any;
  isMobileMenuOpen = false;
  latestBlogs: BlogDto[] = [];
  categoriesWithBlogs: CategoryDto[] = [];

  constructor(
    private blogService: BlogService,
    private categoryService: CategoryService
  ){}

  ngOnInit() {
    this.getLatest5Blogs();
    this.getCategoriesWithBlogs();

    AOS.init({
      duration: 1000,
      easing: 'ease-in-out',
      once: true,
      mirror: false
    });

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

  getLatest5Blogs(){
    this.blogService.getLatest5Blogs().subscribe({
      next: result => this.latestBlogs = Array.isArray(result) ? result : []
    })
  }

  getCategoriesWithBlogs(){
    forkJoin({
      categories: this.categoryService.getCategories(),
      blogs: this.blogService.getAll()
    }).subscribe({
      next: ({ categories, blogs }) => {
        const categoryList = Array.isArray(categories) ? categories : [];
        const blogList = Array.isArray(blogs) ? blogs : [];

        this.categoriesWithBlogs = categoryList
          .map((category: CategoryDto) => ({
            ...category,
            blogs: blogList.filter((blog: BlogDto) => blog?.categoryId === category?.id)
          }))
          .filter((category: CategoryDto) => Array.isArray(category.blogs) && category.blogs.length > 0);
      },
      error: () => {
        this.categoriesWithBlogs = [];
      }
    });
  }
}
