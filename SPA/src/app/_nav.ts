import { INavData } from '@coreui/angular';

export const navItems: INavData[] = [
  {
    name: 'Quản lý quản trị',
    url: '/maintain/nguoi-dung',
    icon: 'fa fa-cogs',
  },
  {
    name: 'Quản lý NLĐ',
    url: '/maintain/khach-hang',
    icon: 'fa fa-users',
  },
  {
    name: 'Quản lý mã hàng',
    url: '/maintain/ma-hang',
    icon: 'fa fa-barcode',
  },
  {
    name: 'Quản lý công đoạn',
    url: '/maintain/cd',
    icon: 'fa fa-steam-square',
  },
  {
    name: 'Chấm công',
    url: '/maintain/mua-hang',
    icon: 'fa fa-calculator',
  },
  {
    name: 'Báo cáo',
    url: '/report',
    icon: 'fa fa-calendar-check-o',
  }
];




