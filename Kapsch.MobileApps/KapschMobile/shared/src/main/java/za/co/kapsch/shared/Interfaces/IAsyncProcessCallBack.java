package za.co.kapsch.shared.Interfaces;

import za.co.kapsch.shared.Models.AsyncResultModel;

/**
 * Created by csenekal on 2016-09-26.
 */
public interface IAsyncProcessCallBack {

    void progressCallBack(AsyncResultModel asyncResultModel);
    void finishedCallBack(AsyncResultModel asyncResultModel);
}
