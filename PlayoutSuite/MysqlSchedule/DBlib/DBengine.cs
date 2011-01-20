using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MysqlSchedule.DBlib
{
    public enum ITEMFILTER { CATEGORY, NONE, CANVASPERIOD, CANVASSLICE, DATAITEMID };
    public interface DBengine
    {

        void connect(String host, String username, String password);
        void connect(String host, String username, String password, String database);
        DataTable listItems(ITEMFILTER filter=ITEMFILTER.NONE, String param="");
        void newItem(Dictionary<DBfield, String> values);
        void modifyItem(int iditem, Dictionary<DBfield, String> values);
        void Update(DataTable DTItems);
        List<DBfield> getFields();
        Boolean isConnected();

        Dictionary<string, DataItemCategory> listCategories();

        List<string> listCategoryTypes();

        List<PeriodCanvas> listPeriodCanvas();

        List<SliceCanvas> listSliceCanvas();

        int newSlice(string p);

        void removeSlice(int p);

        void addNewCanvasItem(int canvasId, int slicePosition, SlotCanvasType slotCanvasType, string param1, string param2, string param3, string label);

        void modifyCanvasItem(int canvasId, int slicePosition, int itemId, SlotCanvasType slotCanvasType, string param1, string param2, string param3, string label);

        void removePeriod(int p);
    }
}
