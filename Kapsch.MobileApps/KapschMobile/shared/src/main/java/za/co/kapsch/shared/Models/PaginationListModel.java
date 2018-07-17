package za.co.kapsch.shared.Models;

/**
 * Created by CSenekal on 2017/07/03.
 */

import com.google.gson.annotations.SerializedName;

import java.util.List;

public class PaginationListModel<T> {

    @SerializedName("HasPreviousPage")
    private boolean mHasPreviousPage;
    @SerializedName("HasNextPage")
    private boolean mHasNextPage;
    @SerializedName("PageIndex")
    private int mPageIndex;
    @SerializedName("PageSize")
    private int mPageSize;
    @SerializedName("TotalPages")
    private int mTotalPages;
    @SerializedName("TotalCount")
    private int mTotalCount;
    @SerializedName("Models")
    private List<T> mModels;

    public boolean getHasPreviousPage() {
        return mHasPreviousPage;
    }

    public boolean getHasNextPage() {
        return mHasNextPage;
    }

    public int getPageIndex() {
        return mPageIndex;
    }

    public int getPageSize() {
        return mPageSize;
    }

    public int getTotalPages() {
        return mTotalPages;
    }

    public int getTotalCount() {
        return mTotalCount;
    }

    public List<T> getModels() {
        return mModels;
    }
}
