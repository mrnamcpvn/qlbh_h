using AgileObjects.AgileMapper;
using AgileObjects.AgileMapper.Extensions;
using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Maintain;
using API.Helper.Mappers;
using API.Helpers.Params;
using API.Models;
using LinqKit;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_DonHang : I_DonHang
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_DonHang(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<DonHang>> GetDataPagination(PaginationParams pagination,string fromDate, string toDate, int type)
        {
            var predicateUser = PredicateBuilder.New<DonHang>(true);

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                predicateUser.And(x => Convert.ToDateTime(fromDate) <= x.Date && x.Date <= Convert.ToDateTime(toDate));
            }
            if(type > 0) {
                predicateUser.And(x => x.Loai == type);
            }
            var donHangs = _repoAccessor.DonHang.FindAll(predicateUser).OrderByDescending(x => x.Date);
            var result = await PaginationUtility<DonHang>.CreateAsync(donHangs, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<DonHang> Create(DonHangDTO model)
        {
            DonHang dh = new DonHang();
            dh.Date = model.Date;
            dh.ID_KH = model.ID_KH;
            dh.Ten_KH = model.Ten_KH;
            dh.TongTien = model.TongTien;
            dh.Loai = model.Loai;
            dh.Date = DateTime.Now;
            _repoAccessor.DonHang.Add(dh);
            await _repoAccessor.Save();
            var idDH = _repoAccessor.DonHang.FindAll().DefaultIfEmpty().Max(r => r == null ? 0 : r.ID);
            foreach(var item in model.ChiTiet)
            {
                var sp = await _repoAccessor.SanPham.FindById(item.ID_SP);
                item.SL_Ton_Dau = sp.SoLuong;
                if(model.Loai == 1)
                    sp.SoLuong = sp.SoLuong== null? 0: sp.SoLuong + item.SoLuong;
                else sp.SoLuong = sp.SoLuong== null? 0: sp.SoLuong - item.SoLuong;
                item.ID_DH = idDH;
                item.SL_Ton_Cuoi = sp.SoLuong;
                item.Dvt = sp.Dvt;
                item.Updated_Time = DateTime.Now;
                _repoAccessor.ChiTietDonHang.Add(item);
                _repoAccessor.SanPham.Update(sp);
            }

            try 
            {
                await _repoAccessor.Save();
                return dh;
            }
            catch
            {
                _repoAccessor.DonHang.Remove(idDH);
                await _repoAccessor.Save();
                return new DonHang();
            }
            
        }

        public async Task<DonHang> Update(DonHangDTO model)
        {
            var dh = await _repoAccessor.DonHang.FindById(model.ID);
            dh.ID_KH = model.ID_KH;
            dh.Ten_KH = model.Ten_KH;
            dh.TongTien = model.TongTien;
            _repoAccessor.DonHang.Update(dh);
            foreach(var item in model.ChiTiet)
            {
                var sp = await _repoAccessor.SanPham.FindById(item.ID_SP);
                var chiTiet = await _repoAccessor.ChiTietDonHang.FindById(item.ID);

                if(chiTiet != null) {
                    if(model.Loai == 1)
                        sp.SoLuong =  sp.SoLuong== null? 0: sp.SoLuong + (item.SoLuong - chiTiet.SoLuong);
                    else sp.SoLuong = sp.SoLuong== null? 0: sp.SoLuong - (item.SoLuong - chiTiet.SoLuong);
                    chiTiet.SL_Ton_Cuoi = sp.SoLuong;
                    chiTiet.Updated_Time = DateTime.Now;
                    chiTiet.SoLuong = item.SoLuong;
                    chiTiet.ThanhTien = item.ThanhTien;
                    chiTiet.Gia = item.Gia;
                    _repoAccessor.ChiTietDonHang.Update(chiTiet);
                }else {
                    item.SL_Ton_Dau = sp.SoLuong== null? 0: sp.SoLuong;
                    if(model.Loai == 1)
                        sp.SoLuong = sp.SoLuong== null? 0: sp.SoLuong + item.SoLuong;
                    else sp.SoLuong = sp.SoLuong== null? 0: sp.SoLuong - item.SoLuong;
                    item.ID_DH = model.ID;
                    item.SL_Ton_Cuoi = sp.SoLuong;
                    item.Dvt = sp.Dvt;
                    item.Updated_Time = DateTime.Now;
                    _repoAccessor.ChiTietDonHang.Add(item);
                }
                _repoAccessor.SanPham.Update(sp);
            }
            try 
            {
                await _repoAccessor.Save();
                return dh;
            }
            catch
            {
                return new DonHang();
            }
        }

        public async Task<bool> Delete(int id)
        {
            var dh = await _repoAccessor.DonHang.FindSingle(x => x.ID == id);
            var listChitiet = await _repoAccessor.ChiTietDonHang.FindAll(x=> x.ID_DH == id).ToListAsync();
            
            if (dh != null)
            {
                foreach (var item in listChitiet)
                {
                    var sp = await _repoAccessor.SanPham.FindById(item.ID_SP);
                    if(dh.Loai == 1) sp.SoLuong -= item.SoLuong;
                    else sp.SoLuong += item.SoLuong;
                    _repoAccessor.ChiTietDonHang.Remove(item);
                }
                _repoAccessor.DonHang.Remove(dh);
                try {
                    return await _repoAccessor.Save();  
                }catch {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<List<ChiTietDonHang>> GetDetail(int id)
        {
            var data = await _repoAccessor.ChiTietDonHang.FindAll(x => x.ID_DH == id).ToListAsync();
            return data;
        }
    }
}