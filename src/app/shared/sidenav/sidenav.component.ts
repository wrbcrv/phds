import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'phds-sidenav',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss'],
})
export class SidenavComponent implements OnInit {
  items = [
    {
      label: 'Home',
      icon: '<i class="fa-solid fa-house"></i>',
      url: '',
      sub: null
    },
    {
      label: 'Chamados',
      icon: '<i class="fa-solid fa-circle-info"></i>',
      url: '/chamados',
      sub: [
        {
          label: 'Chamados Abertos',
          url: '/tickets'
        },
        {
          label: 'Novo Chamado',
          url: '/novo-chamado'
        }
      ]
    },
  ];

  active: any = null;
  isHomeRoute: boolean = false;

  constructor(private router: Router) { }

  ngOnInit(): void {
    this.router.events.subscribe(() => {
      this.isHomeRoute = this.router.url === '/';
    });
  }

  handleClick(item: any) {
    if (item.sub) {
      this.active = this.active === item ? null : item;
    } else if (item.url) {
      this.router.navigate([item.url]);
    }
  }

  handleSubClick(subItem: any) {
    this.router.navigate([subItem.url]);
    this.active = null;
  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (!target.closest('.sidenav-container')) {
      this.active = null;
    }
  }
}
