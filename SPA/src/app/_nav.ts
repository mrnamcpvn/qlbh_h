import { INavData } from '@coreui/angular';

export const navItems: INavData[] = [
  {
    name: 'Quản lý quản trị',
    url: '/maintain/nguoi-dung',
    icon: 'fa fa-cogs',
  },
  {
    name: 'Quản lý khách hàng',
    url: '/maintain/khach-hang',
    icon: 'fa fa-users',
  },
  {
    name: 'Quản lý sản phẩm',
    url: '/maintain/san-pham',
    icon: 'fa fa-barcode',
  },
  {
    name: 'Nhập hàng',
    url: '/maintain/mua-hang',
    icon: 'fa fa-calculator',
  },
  {
    name: 'Xuất hàng',
    url: '/maintain/ban-hang',
    icon: 'fa fa-steam-square',
  },
  {
    name: 'Báo cáo tồn kho',
    url: '/report',
    icon: 'fa fa-calendar-check-o',
  }
];




