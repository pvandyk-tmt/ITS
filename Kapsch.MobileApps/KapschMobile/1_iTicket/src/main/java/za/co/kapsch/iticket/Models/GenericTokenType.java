package za.co.kapsch.iticket.Models;

import com.google.gson.reflect.TypeToken;

import java.lang.reflect.Type;

/**
 * Created by CSenekal on 2017/01/20.
 */
public class GenericTokenType <T> {

    public Type getGenericTokenType() {
        return new TypeToken<T>() {}.getType();
    }
}
