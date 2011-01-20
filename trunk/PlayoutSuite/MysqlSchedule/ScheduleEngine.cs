using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MysqlSchedule.DBlib;
using System.Data;

namespace MysqlSchedule
{
    public class ScheduleEngine
    {
        private DateTime date;
        private string idPeriod;
        private DBengineMySql dbengine;
        public event System.ComponentModel.ProgressChangedEventHandler ProgressChanged;


        public ScheduleEngine(DateTime date, string p)
        {
            // TODO: Complete member initialization
            this.date = date;
            this.idPeriod = p;
            this.dbengine = DBengineMySql.GetInstance();

            
        }

        public void startScheduling(){
            DataTable slices = dbengine.listItems(ITEMFILTER.CANVASPERIOD, this.idPeriod);
            DateTime d = date.Date;
            int nslices = slices.Rows.Count;
            for (int i = 0; i < slices.Rows.Count; i++)
            {
                
                String nameslice = slices.Rows[i]["name"].ToString();
                String idslice = slices.Rows[i]["idcanvasslice"].ToString();

                Console.WriteLine("***"+ slices.Rows[i]["name"]);

                DataTable slots = dbengine.listItems(ITEMFILTER.CANVASSLICE, idslice);
                if (slots.Rows.Count != 0)
                {
                    int idschedslice = dbengine.scheduleNewSlice(nameslice, d);

                    int nslots = slots.Rows.Count;
                    for (int j = 0; j < slots.Rows.Count; j++)
                    {

                        SlotCanvasType slottype = getCanvasTypeFromString(slots.Rows[j]["type"].ToString());
                        String param1 = slots.Rows[j]["param1"].ToString();
                        String param2 = slots.Rows[j]["param2"].ToString();
                        String param3 = slots.Rows[j]["param3"].ToString();

                        if (slottype == SlotCanvasType.SYNC)
                        {
                            dbengine.scheduleNewSyncToSlice(idschedslice, j, d, param1, param2);
                        }
                        else if (slottype == SlotCanvasType.SPECIFICITEM)
                        {
                            d = dbengine.scheduleNewSpecificItemToSlice(idschedslice, j, d, param1);
                        }
                        else if (slottype == SlotCanvasType.CATEGORYITEM)
                        {
                            d = dbengine.scheduleNewCategoryItemToSlice(idschedslice, j, d, param1);
                        }
                        else if (slottype == SlotCanvasType.SLIDESLOAD)
                        {
                            d = dbengine.scheduleNewSlidesLoad(idschedslice, j, d, param1);
                        }
                        ProgressChanged(this, new System.ComponentModel.ProgressChangedEventArgs(i * 100 / nslices + j * 10 / nslots, 1));
                    }
                    

                }

            }

        }

        SlotCanvasType getCanvasTypeFromString(String t)
        {
            if (t == SlotCanvasType.CATEGORYITEM.ToString()) return SlotCanvasType.CATEGORYITEM;
            else if (t == SlotCanvasType.SLIDESLOAD.ToString()) return SlotCanvasType.SLIDESLOAD;
            else if (t == SlotCanvasType.SYNC.ToString()) return SlotCanvasType.SYNC;
            else if (t == SlotCanvasType.SPECIFICITEM.ToString()) return SlotCanvasType.SPECIFICITEM;
            else return SlotCanvasType.UNKNOWN;

        }


    }
}
