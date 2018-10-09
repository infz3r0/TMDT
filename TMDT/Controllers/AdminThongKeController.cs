using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using PagedList;

namespace TMDT.Controllers
{
    public class AdminThongKeController : Controller
    {
        private TMDTDBDataContext data = new TMDTDBDataContext();

        public ActionResult Index()
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }

        #region SanPham
        public ActionResult SanPham(int? page)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            List<MonthYear> distinct = data.V_ThongKeSanPhams.Select(i => new MonthYear((int)i.Thang, (int)i.Nam)).Distinct().ToList();
            distinct.Reverse();

            int pageSize = 12;
            int pageNum = page ?? 1;
            return View(distinct.ToPagedList(pageNum, pageSize));
        }

        [ChildActionOnly]
        //Dùng trong Index
        public ActionResult PV_ThongKeSanPham()
        {
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            ViewBag.Month = month;
            ViewBag.Year = year;

            CaiDat config = data.CaiDats.SingleOrDefault(i => i.TenThamSo.Equals("top_san_pham_take"));
            int take = Convert.ToInt32(config.GiaTri);
            ViewBag.Take = take;

            List<V_ThongKeSanPham> all = data.V_ThongKeSanPhams.Where(i => i.Thang == month && i.Nam == year).OrderByDescending(i => i.TongSoLuong).Take(take).ToList();

            string list_labels = "[";
            foreach (V_ThongKeSanPham item in all)
            {
                if (all.IndexOf(item) == all.Count - 1)
                {
                    list_labels += "'" + item.TenSanPham + "'";
                }
                else
                {
                    list_labels += "'" + item.TenSanPham + "'" + ",";
                }
            }
            list_labels += "]";
            ViewBag.List_labels = list_labels;

            string list_data = "[";
            foreach (V_ThongKeSanPham item in all)
            {
                if (all.IndexOf(item) == all.Count - 1)
                {
                    list_data += "" + item.TongSoLuong + "";
                }
                else
                {
                    list_data += "" + item.TongSoLuong + "" + ",";
                }
            }
            list_data += "]";
            ViewBag.List_data = list_data;

            return PartialView(all);
        }

        [ChildActionOnly]
        //Dùng trong SanPham
        public ActionResult PV_SanPhamByMonthYear(int month, int year)
        {
            List<V_ThongKeSanPham> all = data.V_ThongKeSanPhams.Where(i => i.Thang == month && i.Nam == year).OrderByDescending(i => i.TongSoLuong).ToList();
            return PartialView(all);
        }
        #endregion
        #region User
        public ActionResult TaiKhoan(int? page)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            List<MonthYear> distinct = data.V_ThongKeUsers.Select(i => new MonthYear((int)i.Thang, (int)i.Nam)).Distinct().ToList();
            distinct.Reverse();

            int pageSize = 12;
            int pageNum = page ?? 1;
            return View(distinct.ToPagedList(pageNum, pageSize));
        }


        [ChildActionOnly]
        //Dùng trong Index
        public ActionResult PV_ThongKeUser()
        {
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            ViewBag.Month = month;
            ViewBag.Year = year;

            CaiDat config = data.CaiDats.SingleOrDefault(i => i.TenThamSo.Equals("top_tai_khoan_take"));
            int take = Convert.ToInt32(config.GiaTri);
            ViewBag.Take = take;

            List<V_ThongKeUser> all = data.V_ThongKeUsers.Where(i => i.Thang == month && i.Nam == year).OrderByDescending(i => i.TongThanhTien).Take(take).ToList();

            string list_labels = "[";
            foreach (V_ThongKeUser item in all)
            {
                if (all.IndexOf(item) == all.Count - 1)
                {
                    list_labels += "'" + item.Username + "'";
                }
                else
                {
                    list_labels += "'" + item.Username + "'" + ",";
                }
            }
            list_labels += "]";
            ViewBag.List_labels = list_labels;

            string list_data_TongThanhTien = "[";
            foreach (V_ThongKeUser item in all)
            {
                if (all.IndexOf(item) == all.Count - 1)
                {
                    list_data_TongThanhTien += "" + item.TongThanhTien + "";
                }
                else
                {
                    list_data_TongThanhTien += "" + item.TongThanhTien + "" + ",";
                }
            }
            list_data_TongThanhTien += "]";
            ViewBag.List_data_TongThanhTien = list_data_TongThanhTien;

            return PartialView(all);
        }

        [ChildActionOnly]
        //Dùng trong TaiKhoan
        public ActionResult PV_TaiKhoanByMonthYear(int month, int year)
        {
            List<V_ThongKeUser> all = data.V_ThongKeUsers.Where(i => i.Thang == month && i.Nam == year).OrderByDescending(i => i.TongThanhTien).ToList();
            return PartialView(all);
        }

        #endregion
        #region DoanhThu
        public ActionResult DoanhThu(int? page)
        {
            if (!Manager.LoggedAsAdmin())
            {
                return RedirectToAction("Login", "Admin");
            }

            List<V_ThongKeDoanhThu> distinct = data.V_ThongKeDoanhThus.ToList();
            distinct.Reverse();

            int pageSize = 12;
            int pageNum = page ?? 1;
            return View(distinct.ToPagedList(pageNum, pageSize));
        }

        [ChildActionOnly]
        //Dùng trong Index
        public ActionResult PV_ThongKeDoanhThu()
        {
            int takeMonth = 12;

            List<V_ThongKeDoanhThu> all = data.V_ThongKeDoanhThus.OrderByDescending(i => i.Nam).OrderByDescending(i => i.Thang).Take(takeMonth).ToList();
            all.Reverse();

            string textMonth = "Tháng";
            int year = 0;
            string list_labels = "[";
            foreach (V_ThongKeDoanhThu item in all)
            {
                if (item.Nam != year)
                {
                    year = (int)item.Nam;
                    list_labels += string.Format("['{0} {1}', '{2}']", textMonth, item.Thang, year);
                }
                else
                {
                    list_labels += "'" + item.Thang + "'";
                }

                if (all.IndexOf(item) != all.Count - 1)
                {
                    list_labels += ",";
                }
            }
            list_labels += "]";
            ViewBag.List_labels = list_labels;

            string list_data = "[";
            foreach (V_ThongKeDoanhThu item in all)
            {
                if (all.IndexOf(item) == all.Count - 1)
                {
                    list_data += "" + item.TongDoanhThu + "";
                }
                else
                {
                    list_data += "" + item.TongDoanhThu + "" + ",";
                }
            }
            list_data += "]";
            ViewBag.List_data = list_data;

            return PartialView(all);
        }
        #endregion

    }
}