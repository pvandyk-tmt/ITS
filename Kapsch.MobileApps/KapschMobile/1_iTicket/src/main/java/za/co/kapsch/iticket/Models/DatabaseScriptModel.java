package za.co.kapsch.iticket.Models;

/**
 * Created by csenekal on 2016-09-13.
 */
public class DatabaseScriptModel {
    private long Id;
    private String Script;

    public long getId() {
        return Id;
    }

    public void setId(long id) {
        Id = id;
    }

    public String getScript() {
        return Script;
    }

    public void setScript(String script) {
        Script = script;
    }
}
