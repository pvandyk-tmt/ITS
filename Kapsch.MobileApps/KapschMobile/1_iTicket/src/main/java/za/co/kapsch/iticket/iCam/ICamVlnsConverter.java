package za.co.kapsch.iticket.iCam;

import com.thoughtworks.xstream.converters.Converter;
import com.thoughtworks.xstream.converters.MarshallingContext;
import com.thoughtworks.xstream.converters.UnmarshallingContext;
import com.thoughtworks.xstream.io.HierarchicalStreamReader;
import com.thoughtworks.xstream.io.HierarchicalStreamWriter;

/**
 * Created by csenekal on 2016-12-13.
 */

public class ICamVlnsConverter implements Converter {

    public boolean canConvert(Class clazz) {
        return ICamVlns.class == clazz;
    }

    public void marshal(Object object, HierarchicalStreamWriter hsw, MarshallingContext mc) {

        ICamVlns iCamVlns = (ICamVlns) object;
        hsw.addAttribute("timestamp", iCamVlns.getTimestamp());
        hsw.addAttribute("date", iCamVlns.getDate());
        hsw.addAttribute("time", iCamVlns.getTime());
        hsw.addAttribute("serialnum", iCamVlns.getSerialnum());
        hsw.addAttribute("hwid", iCamVlns.getHwid());
        hsw.addAttribute("plates", iCamVlns.getPlates());
        hsw.addAttribute("filename", iCamVlns.getFilename());

//            for (Map.Entry<String, String> entry : e.AnyAttr.entrySet())
//            {
//                hsw.addAttribute(entry.getKey(), entry.getValue());
//            }
    }

    public Object unmarshal(HierarchicalStreamReader hsr, UnmarshallingContext uc) {

        ICamVlns iCamVlns = new ICamVlns();
        String key;
        int count = hsr.getAttributeCount();

        for (int i = 0; i < count; i++)
        {
            key = hsr.getAttributeName(i);
            if  (key.equals("timestamp")) iCamVlns.setTimestamp(hsr.getAttribute(i));
            else if (key.equals("date")) iCamVlns.setDate(hsr.getAttribute(i));
            else if (key.equals("time")) iCamVlns.setTime(hsr.getAttribute(i));
            else if (key.equals("serialnum")) iCamVlns.setSerialnum(hsr.getAttribute(i));
            else if (key.equals("hwid")) iCamVlns.setHwid(hsr.getAttribute(i));
            else if (key.equals("plates")) iCamVlns.setPlates(hsr.getAttribute(i));
            else if (key.equals("filename")) iCamVlns.setFilename(hsr.getAttribute(i));

//                else ICamVlns.AnyAttr.put(key, hsr.getAttribute(i));
        }
        return iCamVlns;
    }
}
