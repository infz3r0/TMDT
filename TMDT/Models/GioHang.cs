using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class GioHang
    {
        TMDTDBDataContext data = new TMDTDBDataContext();
        public string sHinhAnh { set; get; }
        public double dPhiVanChuyen { set; get; }
        public int iIDSanPham { set; get; }
        public int iIDNSX { set; get; }
        public string sTenSanPham { set; get; }
        public double dDonGia { set; get; }
        public int iSoLuong { set; get; }
        public int iID { set; get; }
        public double dThanhTien
        {
            get { return (dDonGia * iSoLuong) + dPhiVanChuyen; }
        }
        //khoi tao gio hang theo ma thuc don duoc truyen vao
        public GioHang(int IDSanPham)
        {
            iIDSanPham = IDSanPham;
            SanPham td = data.SanPhams.Single(n => n.IDSanPham == iIDSanPham);
            dPhiVanChuyen = double.Parse(td.PhiVanChuyen.ToString());
            sTenSanPham = td.TenSanPham;
            sHinhAnh = td.HinhAnh;
            dDonGia = double.Parse(td.DonGia.ToString());
            iSoLuong = 1;
        }
    }
}