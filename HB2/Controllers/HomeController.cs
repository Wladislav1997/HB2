using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HB2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HB2.ViewModels;


namespace HB.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        UserContext db;

        public HomeController(UserContext user)
        {
            db = user;
        }
        [HttpGet]
        public IActionResult OpAc()
        {
            return View();
        }
        [HttpPost]
        public IActionResult OpAc(P1 p1)
        {
            db.P1s.Add(p1);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DelOp(int id)
        {
            Operation op = db.Operations.FirstOrDefault(p => p.Id == id);
            db.Operations.Remove(op);
            db.SaveChanges();
            return RedirectToAction("OperPlan", new { id = op.PlanId });
            
        }
        public IActionResult DelPlan(int id)
        {
            Plan plan = db.Plans.FirstOrDefault(p => p.Id == id);
            db.Plans.Remove(plan);
            db.SaveChanges();
            return RedirectToAction("Plan");

        }
        public IActionResult Index()
        {
            IQueryable<Operation> operat = db.Operations.Include(c => c.p).Include(u=>u.Plan);
            operat = operat.Where(p => p.Plan.User.Email == User.Identity.Name);
            operat = operat.Where(p => p.Plan.Data <= DateTime.Now && p.Plan.DataPeriod >= DateTime.Now);
            foreach(Operation op in operat)
            {
                int pr = op.Sum / 100;// 1%
                int s = 0;
                foreach (P p in op.p)
                {
                    s += p.Sum;
                }
                op.SumP = s;
                op.Procent = s / pr;
                db.Operations.Update(op);
               
            }
            db.SaveChanges();
            Index_1 ind = new Index_1();
            ind.operat = operat;
           
            //IQueryable<P> ps = db.Ps.Include(c => c.Operation);
            //ps = ps.Where(p => p.Operation.Plan.User.Email == User.Identity.Name);// все совершонные плановые операции юзера

            //IQueryable<P1> p1s = db.P1s.Include(c => c.User);
            //p1s = p1s.Where(p => p.User.Email == User.Identity.Name);// все совершонные внеплановые операции юзера

            //IQueryable<P> _ps = ps.Where(p => p.Operation.NameAct == "расход");
            //foreach (P p in _ps)
            //{
            //    ind.RasMon += p.Sum;
            //}

            //IQueryable<P1> __ps = p1s.Where(p => p.NameAct == "расход");
            //foreach (P1 p in __ps)
            //{
            //    ind.RasMon += p.Sum;
            //}

            //IQueryable<P> _psd = ps.Where(p => p.Operation.NameAct == "доход");
            //foreach (P p in _psd)
            //{
            //    ind.DochMon += p.Sum;
            //}

            //IQueryable<P1> __psd = p1s.Where(p => p.NameAct == "доход");
            //foreach (P1 p in __psd)
            //{
            //    ind.DochMon += p.Sum;
            //}

            //ind.RazDochRas = ind.DochMon - ind.RasMon;

            return View(ind);
        }
        [HttpGet]
        public IActionResult Make(int id)
        {
            Operation op = db.Operations.FirstOrDefault(p => p.Id == id);
            Mod1 mod1 = new Mod1();
            mod1.Id = id;
            mod1.Operation = op;
            return View(mod1);
        }
        [HttpPost]
        public IActionResult Make(P p)
        {

            db.Ps.Add(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
       
        public IActionResult Plan()
        {
            IQueryable<Plan> pl = db.Plans.Include(c => c.User).Include(u => u.Operations).ThenInclude(u => u.p);
            pl = pl.Where(p => p.User.Email == User.Identity.Name);
            pl = pl.Where(p => p.DataPeriod >= DateTime.Now);// только актульные планы дата окончания которых либо больше либо равна времени на компе
            foreach(Plan p in pl)
            {
                int i = 0;
                int t=0;
                int ras = 0;
                int doch = 0;
                foreach(Operation op in p.Operations)
                {
                    if (op.NameAct == "доход")
                    {
                        doch += op.SumP;
                    }
                    else
                    {
                        ras += op.SumP;
                    }
                    i += op.Procent;
                    t++;
                }
                p.RasMonth = ras;
                p.DochMonth = doch;
                p.RaznDochRas = doch - ras;
                if (i != 0)
                {
                    p.Procent = i / t;
                }
                else
                {
                    p.Procent = 0;
                }
                db.Plans.Update(p);
            }
            db.SaveChanges();
            return View(pl);
        }

        public IActionResult OperPlan(int id)
        {
            IQueryable<Operation> operat = db.Operations.Include(c => c.Plan);
            operat = operat.Where(p => p.PlanId == id);
            RedactPl pl = new RedactPl();
            pl.Opers = operat;
            pl._Id = id;
            return View(pl);
        }
        [HttpGet]
        public IActionResult RedactPlan(int id)
        {
            Plan pl = db.Plans.FirstOrDefault(p => p.Id == id);
            return View(pl);
        }
        [HttpPost]
        public IActionResult RedactPlan(Plan plan)
        {
            db.Plans.Update(plan);
            db.SaveChanges();
            return RedirectToAction("Plan");
        }
        public IActionResult AddPlan()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddPlan(Plan plan)
        {
            db.Plans.Add(plan);
            db.SaveChanges();
            return RedirectToAction("Plan");
        }
        [HttpGet]
        public IActionResult RedactOper(int id)
        {
            Operation op = db.Operations.FirstOrDefault(p => p.Id == id);

            return View(op);
        }
        [HttpPost]
        public IActionResult RedactOper(Operation op)
        {
            db.Operations.Update(op);
            db.SaveChanges();
            return RedirectToAction("OperPlan", new { id = op.PlanId });
        }
        [HttpGet]
        public IActionResult AddOperation(int id)
        {
            Id id1 = new Id();
            id1._Id = id;
            return View(id1);
        }
        [HttpPost]
        public IActionResult AddOperation(Operation op)
        {

            db.Operations.Add(op);
            db.SaveChanges();
            return RedirectToAction("OperPlan", new { id = op.PlanId });
        }

        public IActionResult Filter()
        {
            return View();
        }

        public IActionResult SovOper(int id)
        {
            IQueryable<P> ps = db.Ps.Include(c => c.Operation);
            ps = ps.Where(p => p.OperationId == id);
            RedactPl pl = new RedactPl();
            pl.ps = ps;
            Operation op = db.Operations.FirstOrDefault(p => p.Id == id);
            pl._Id = op.PlanId;
            return View(pl);
        }
        public IActionResult SovOpInd(int id)
        {
            IQueryable<P> ps = db.Ps.Include(c => c.Operation);
            ps = ps.Where(p => p.OperationId == id);
            return View(ps);
        }
        [HttpGet]
        public IActionResult RedDelOp(int id)
        {
            Operation op = db.Operations.FirstOrDefault(p => p.Id == id);
            return View(op);
        }
        [HttpPost]
        public IActionResult RedDelOp(Operation op)
        {
            return View();
        }
        public IActionResult DelOpInd(int id)
        {
            Operation op = db.Operations.FirstOrDefault(p => p.Id == id);
            db.Operations.Remove(op);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult ArchPl(ArchPl apl)
        {
            TimeSpan minper = new TimeSpan(apl.Days,apl.Clock,apl.Min,0);
            TimeSpan maxper = new TimeSpan(apl._Days, apl._Clock, apl._Min, 0);
            TimeSpan per = new TimeSpan(0, 0, 0, 0);
            DateTime date = new DateTime(2000, 1, 1, 0, 0,0);
            IQueryable<Plan> pl = db.Plans.Include(c => c.User);
            pl = pl.Where(p => p.User.Email == User.Identity.Name);
            pl = pl.Where(a => a.DataPeriod < DateTime.Now);
            ArchPl ap = new ArchPl();
            if (apl.Name != null)
            {
                pl = pl.Where(a => a.Name == apl.Name);
                if (apl.mindoch != 0 && apl.maxdoch!=0)
                {
                    pl = pl.Where(p => p.DochMonth >= apl.mindoch && p.DochMonth <= apl.maxdoch);
                    if (apl.minras != 0 && apl.maxras != 0)
                    {
                        pl = pl.Where(p => p.RasMonth >= apl.minras && p.RasMonth <= apl.maxras);
                        if (apl.minit != 0 && apl.maxit != 0)
                        {
                            pl = pl.Where(p => p.RaznDochRas >= apl.minit && p.RaznDochRas <= apl.maxit);
                            if (apl.minpr != 0 && apl.maxpr != 0)
                            {
                                pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                                if (apl.mind != date && apl.maxd != date)
                                {
                                    pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                                    if (apl.mindp != date && apl.maxdp != date)
                                    {
                                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                        if (minper != per && maxper != per)
                                        {
                                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                            ap.pl = pl;
                                            return View(ap);
                                        }
                                        ap.pl = pl;
                                        return View(ap);
                                    }
                                    if (minper != per && maxper != per)
                                    {
                                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                        ap.pl = pl;
                                        return View(ap);
                                    }
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                if (apl.mindp != date && apl.maxdp != date)
                                {
                                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                    if (minper != per && maxper != per)
                                    {
                                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                        ap.pl = pl;
                                        return View(ap);
                                    }
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (apl.mind != date && apl.maxd != date)
                            {
                                pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                                if (apl.mindp != date && apl.maxdp != date)
                                {
                                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                    if (minper != per && maxper != per)
                                    {
                                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                        ap.pl = pl;
                                        return View(ap);
                                    }
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.minpr != 0 && apl.maxpr != 0)
                        {
                            pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                            if (apl.mind != date && apl.maxd != date)
                            {
                                pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                                if (apl.mindp != date && apl.maxdp != date)
                                {
                                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                    if (minper != per && maxper != per)
                                    {
                                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                        ap.pl = pl;
                                        return View(ap);
                                    }
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mind != date && apl.maxd != date)
                        {
                            pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.minit != 0 && apl.maxit != 0)
                    {
                        pl = pl.Where(p => p.RaznDochRas >= apl.minit && p.RaznDochRas <= apl.maxit);
                        if (apl.minpr != 0 && apl.maxpr != 0)
                        {
                            pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                            if (apl.mind != date && apl.maxd != date)
                            {
                                pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                                if (apl.mindp != date && apl.maxdp != date)
                                {
                                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                    if (minper != per && maxper != per)
                                    {
                                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                        ap.pl = pl;
                                        return View(ap);
                                    }
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mind != date && apl.maxd != date)
                        {
                            pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.minpr != 0 && apl.maxpr != 0)
                    {
                        pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                        if (apl.mind != date && apl.maxd != date)
                        {
                            pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mind != date && apl.maxd != date)
                    {
                        pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.minras != 0 && apl.maxras != 0)
                {
                    pl = pl.Where(p => p.RasMonth >= apl.minras && p.RasMonth <= apl.maxras);
                    if (apl.minit != 0 && apl.maxit != 0)
                    {
                        pl = pl.Where(p => p.RaznDochRas >= apl.minit && p.RaznDochRas <= apl.maxit);
                        if (apl.minpr != 0 && apl.maxpr != 0)
                        {
                            pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                            if (apl.mind != date && apl.maxd != date)
                            {
                                pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                                if (apl.mindp != date && apl.maxdp != date)
                                {
                                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                    if (minper != per && maxper != per)
                                    {
                                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                        ap.pl = pl;
                                        return View(ap);
                                    }
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mind != date && apl.maxd != date)
                        {
                            pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.minpr != 0 && apl.maxpr != 0)
                    {
                        pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                        if (apl.mind != date && apl.maxd != date)
                        {
                            pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mind != date && apl.maxd != date)
                    {
                        pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.minit != 0 && apl.maxit != 0)
                {
                    pl = pl.Where(p => p.RaznDochRas >= apl.minit && p.RaznDochRas <= apl.maxit);
                    if (apl.minpr != 0 && apl.maxpr != 0)
                    {
                        pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                        if (apl.mind != date && apl.maxd != date)
                        {
                            pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mind != date && apl.maxd != date)
                    {
                        pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.minpr != 0 && apl.maxpr != 0)
                {
                    pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                    if (apl.mind != date && apl.maxd != date)
                    {
                        pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.mind != date && apl.maxd != date)
                {
                    pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.mindp != date && apl.maxdp != date)
                {
                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (minper != per && maxper != per)
                {
                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                    ap.pl = pl;
                    return View(ap);
                }
                ap.pl = pl;
                return View(ap);
            }
            if (apl.mindoch != 0 && apl.maxdoch != 0)
            {
                pl = pl.Where(p => p.DochMonth >= apl.mindoch && p.DochMonth <= apl.maxdoch);
                if (apl.minras != 0 && apl.maxras != 0)
                {
                    pl = pl.Where(p => p.RasMonth >= apl.minras && p.RasMonth <= apl.maxras);
                    if (apl.minit != 0 && apl.maxit != 0)
                    {
                        pl = pl.Where(p => p.RaznDochRas >= apl.minit && p.RaznDochRas <= apl.maxit);
                        if (apl.minpr != 0 && apl.maxpr != 0)
                        {
                            pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                            if (apl.mind != date && apl.maxd != date)
                            {
                                pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                                if (apl.mindp != date && apl.maxdp != date)
                                {
                                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                    if (minper != per && maxper != per)
                                    {
                                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                        ap.pl = pl;
                                        return View(ap);
                                    }
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mind != date && apl.maxd != date)
                        {
                            pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.minpr != 0 && apl.maxpr != 0)
                    {
                        pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                        if (apl.mind != date && apl.maxd != date)
                        {
                            pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mind != date && apl.maxd != date)
                    {
                        pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.minit != 0 && apl.maxit != 0)
                {
                    pl = pl.Where(p => p.RaznDochRas >= apl.minit && p.RaznDochRas <= apl.maxit);
                    if (apl.minpr != 0 && apl.maxpr != 0)
                    {
                        pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                        if (apl.mind != date && apl.maxd != date)
                        {
                            pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mind != date && apl.maxd != date)
                    {
                        pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.minpr != 0 && apl.maxpr != 0)
                {
                    pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                    if (apl.mind != date && apl.maxd != date)
                    {
                        pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.mind != date && apl.maxd != date)
                {
                    pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.mindp != date && apl.maxdp != date)
                {
                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (minper != per && maxper != per)
                {
                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                    ap.pl = pl;
                    return View(ap);
                }
                ap.pl = pl;
                return View(ap);
            }
            if (apl.minras != 0 && apl.maxras != 0)
            {
                pl = pl.Where(p => p.RasMonth >= apl.minras && p.RasMonth <= apl.maxras);
                if (apl.minit != 0 && apl.maxit != 0)
                {
                    pl = pl.Where(p => p.RaznDochRas >= apl.minit && p.RaznDochRas <= apl.maxit);
                    if (apl.minpr != 0 && apl.maxpr != 0)
                    {
                        pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                        if (apl.mind != date && apl.maxd != date)
                        {
                            pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                            if (apl.mindp != date && apl.maxdp != date)
                            {
                                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                                if (minper != per && maxper != per)
                                {
                                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                    ap.pl = pl;
                                    return View(ap);
                                }
                                ap.pl = pl;
                                return View(ap);
                            }
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mind != date && apl.maxd != date)
                    {
                        pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.minpr != 0 && apl.maxpr != 0)
                {
                    pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                    if (apl.mind != date && apl.maxd != date)
                    {
                        pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.mind != date && apl.maxd != date)
                {
                    pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.mindp != date && apl.maxdp != date)
                {
                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (minper != per && maxper != per)
                {
                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                    ap.pl = pl;
                    return View(ap);
                }
                ap.pl = pl;
                return View(ap);
            }
            if (apl.minit != 0 && apl.maxit != 0)
            {
                pl = pl.Where(p => p.RaznDochRas >= apl.minit && p.RaznDochRas <= apl.maxit);
                if (apl.minpr != 0 && apl.maxpr != 0)
                {
                    pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                    if (apl.mind != date && apl.maxd != date)
                    {
                        pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                        if (apl.mindp != date && apl.maxdp != date)
                        {
                            pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                            if (minper != per && maxper != per)
                            {
                                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                                ap.pl = pl;
                                return View(ap);
                            }
                            ap.pl = pl;
                            return View(ap);
                        }
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.mind != date && apl.maxd != date)
                {
                    pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.mindp != date && apl.maxdp != date)
                {
                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (minper != per && maxper != per)
                {
                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                    ap.pl = pl;
                    return View(ap);
                }
                ap.pl = pl;
                return View(ap);
            }
            if (apl.minpr != 0 && apl.maxpr != 0)
            {
                pl = pl.Where(p => p.Procent >= apl.minpr && p.Procent <= apl.maxpr);
                if (apl.mind != date && apl.maxd != date)
                {
                    pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                    if (apl.mindp != date && apl.maxdp != date)
                    {
                        pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                        if (minper != per && maxper != per)
                        {
                            pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                            ap.pl = pl;
                            return View(ap);
                        }
                        ap.pl = pl;
                        return View(ap);
                    }
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (apl.mindp != date && apl.maxdp != date)
                {
                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (minper != per && maxper != per)
                {
                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                    ap.pl = pl;
                    return View(ap);
                }
                ap.pl = pl;
                return View(ap);
            }
            if (apl.mind != date && apl.maxd != date)
            {
                pl = pl.Where(p => p.Data >= apl.mind && p.Data <= apl.maxd);
                if (apl.mindp != date && apl.maxdp != date)
                {
                    pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                    if (minper != per && maxper != per)
                    {
                        pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                        ap.pl = pl;
                        return View(ap);
                    }
                    ap.pl = pl;
                    return View(ap);
                }
                if (minper != per && maxper != per)
                {
                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                    ap.pl = pl;
                    return View(ap);
                }
                ap.pl = pl;
                return View(ap);
            }
            if (apl.mindp != date && apl.maxdp != date)
            {
                pl = pl.Where(p => p.DataPeriod >= apl.mindp && p.DataPeriod <= apl.maxdp);
                if (minper != per && maxper != per)
                {
                    pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                    ap.pl = pl;
                    return View(ap);
                }
                ap.pl = pl;
                return View(ap);
            }
            if (minper != per && maxper != per)
            {
                pl = pl.Where(p => p.Period >= minper && p.Period <= maxper);
                ap.pl = pl;
                return View(ap);
            }
            ap.pl = pl;
            return View(ap);
        }
    }
}
