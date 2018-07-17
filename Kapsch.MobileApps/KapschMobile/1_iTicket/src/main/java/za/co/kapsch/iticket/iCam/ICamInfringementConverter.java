package za.co.kapsch.iticket.iCam;

import com.thoughtworks.xstream.converters.Converter;
import com.thoughtworks.xstream.converters.MarshallingContext;
import com.thoughtworks.xstream.converters.UnmarshallingContext;
import com.thoughtworks.xstream.io.HierarchicalStreamReader;
import com.thoughtworks.xstream.io.HierarchicalStreamWriter;

/**
 * Created by csenekal on 2016-12-14.
 */
public class ICamInfringementConverter  implements Converter {

    public boolean canConvert(Class clazz) {
        return ICamInfringement.class == clazz;
    }

    public void marshal(Object object, HierarchicalStreamWriter hsw, MarshallingContext mc) {
        ICamInfringement iCamInfringement = (ICamInfringement) object;
        hsw.addAttribute("timestamp", iCamInfringement.getTimestamp());
        hsw.addAttribute("date", iCamInfringement.getDate());
        hsw.addAttribute("time", iCamInfringement.getTime());
        hsw.addAttribute("speed", iCamInfringement.getSpeed());
        hsw.addAttribute("direction", iCamInfringement.getDirection());
        hsw.addAttribute("location", iCamInfringement.getLocation());
        hsw.addAttribute("highspeed", iCamInfringement.getHighspeed());
        hsw.addAttribute("operator", iCamInfringement.getOperator());
        hsw.addAttribute("serialnum", iCamInfringement.getSerialnum());
        hsw.addAttribute("hwid", iCamInfringement.getHwid());
        hsw.addAttribute("distance", iCamInfringement.getDistance());
        hsw.addAttribute("zone", iCamInfringement.getZone());
        hsw.addAttribute("class", iCamInfringement.getVehicleClass());
        hsw.addAttribute("filename", iCamInfringement.getFilename());
        hsw.addAttribute("img-num", iCamInfringement.getImageNumber());
        hsw.addAttribute("type", iCamInfringement.getType());
    }

    public Object unmarshal(HierarchicalStreamReader hsr, UnmarshallingContext uc) {

        ICamInfringement iCamInfringement = new ICamInfringement();
        String key;
        int count = hsr.getAttributeCount();

        for (int i = 0; i < count; i++)
        {
            key = hsr.getAttributeName(i);
            if  (key.equals("timestamp")) iCamInfringement.setTimestamp(hsr.getAttribute(i));
            else if (key.equals("date")) iCamInfringement.setDate(hsr.getAttribute(i));
            else if (key.equals("time")) iCamInfringement.setTime(hsr.getAttribute(i));
            else if (key.equals("speed")) iCamInfringement.setSpeed(hsr.getAttribute(i));
            else if (key.equals("direction")) iCamInfringement.setDirection(hsr.getAttribute(i));
            else if (key.equals("location")) iCamInfringement.setLocation(hsr.getAttribute(i));
            else if (key.equals("highspeed")) iCamInfringement.setHighspeed(hsr.getAttribute(i));
            else if (key.equals("operator")) iCamInfringement.setOperator(hsr.getAttribute(i));
            else if (key.equals("serialnum")) iCamInfringement.setSerialnum(hsr.getAttribute(i));
            else if (key.equals("hwid")) iCamInfringement.setHwid(hsr.getAttribute(i));
            else if (key.equals("distance")) iCamInfringement.setDistance(hsr.getAttribute(i));
            else if (key.equals("zone")) iCamInfringement.setZone(hsr.getAttribute(i));
            else if (key.equals("class")) iCamInfringement.setVehicleClass(hsr.getAttribute(i));
            else if (key.equals("filename")) iCamInfringement.setFilename(hsr.getAttribute(i));
            else if (key.equals("img-num")) iCamInfringement.setImageNumber(hsr.getAttribute(i));
            else if (key.equals("type")) iCamInfringement.setType(hsr.getAttribute(i));
        }

        return iCamInfringement;
    }
}
