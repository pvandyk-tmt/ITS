package za.co.kapsch.console.General;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.util.DisplayMetrics;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;

import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.Models.ActivityLaunchInfoModel;
import za.co.kapsch.console.Models.MobileDeviceApplicationModel;
import za.co.kapsch.console.R;
import za.co.kapsch.console.orm.ConfigItemRepository;
import za.co.kapsch.console.orm.DistrictRepository;
import za.co.kapsch.shared.Constants;
import za.co.kapsch.shared.Enums.PaymentContext;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Models.SystemFunctionModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/07/11.
 */
public class ClientAreaBuilder {

    private static final int ITEM_SIZE = 150;

    private static final int MIN_ITEM_SIZE = 100;
    private static final int MARGIN_SPACING = 30;
    private static final int CLIENT_AREA_MARGIN = 18;
    private static final String SESSION_MODEL = "SessionModel";
    private static final String LAUNCH_ICON_NAME = "launch_icon";

    private UserModel mUser;
    private DistrictModel mDistrict;
    private MobileDeviceModel mMobileDevice;
    private Activity mActivity;
    private int mMaxItemsPerRow;
    private LinearLayout mClientAreaLinearLayout;

    public ClientAreaBuilder(Activity activity, LinearLayout clientAreaLinearLayout, UserModel user, DistrictModel district, MobileDeviceModel mobileDevice){

        mUser = user;
        mDistrict = district;
        mMobileDevice = mobileDevice;
        mActivity = activity;
        mClientAreaLinearLayout = clientAreaLinearLayout;
        mMaxItemsPerRow = calculateMaxItemsPerRow();
    }

    public void run(List<MobileDeviceApplicationModel> applications){

        int index = 0;

        validateUserAccess(applications);

        int applicationCount = applications.size();

        int clientAreaRowCount = getClientAreaRowCount(applicationCount);

        for(int i = 0; i < clientAreaRowCount; i ++ ){

            LinearLayout clientAreaRowLinearLayout = getClientAreaRowLinearLayout();

            int clientAreaRowItemCount = getClientAreaItemCount(applicationCount, i);

            for(int j = 0; j < clientAreaRowItemCount; j++) {
                clientAreaRowLinearLayout.addView(
                        getImageView(
                                getBitmapFromApplication(
                                        applications.get(index).getName(),
                                        LAUNCH_ICON_NAME),
                                applications.get(index).getName(),
                                (int)applications.get(index).getID(),
                                j == clientAreaRowItemCount-1 ? 0 : MARGIN_SPACING));

                index++;
            }

            mClientAreaLinearLayout.addView(clientAreaRowLinearLayout);
        }
    }

    private void validateUserAccess(List<MobileDeviceApplicationModel> applications){

        List<MobileDeviceApplicationModel> removeList = new ArrayList<>();

        for(MobileDeviceApplicationModel mobileDeviceApplication : applications) {
            if (Utilities.validateUserAccess(mUser, mobileDeviceApplication.getName()) == false) {
                removeList.add(mobileDeviceApplication);
            }
        }

        applications.removeAll(removeList);
    }

    private int calculateMaxItemsPerRow(){

        int clientAreaWidth = clientAreaWidth();

        int itemsPerRow = clientAreaWidth / ITEM_SIZE;

        while (itemsPerRow * ITEM_SIZE  + (dpToPx(MARGIN_SPACING) * (itemsPerRow-1)) > clientAreaWidth){
            itemsPerRow--;
        }

        return itemsPerRow;
    }

    private int clientAreaWidth() {

        int clientAreaPixelMargin = dpToPx(CLIENT_AREA_MARGIN);
        DisplayMetrics displaymetrics = new DisplayMetrics();
        mActivity.getWindowManager().getDefaultDisplay().getMetrics(displaymetrics);
        return displaymetrics.widthPixels - (clientAreaPixelMargin * 2);
    }

    public static int dpToPx(int dp) {

        return (int) (dp * Resources.getSystem().getDisplayMetrics().density);
    }

    private int getClientAreaItemCount(int applicationCount, int currentRow){

        int remainingItems = applicationCount - (currentRow * mMaxItemsPerRow);

        if (remainingItems > mMaxItemsPerRow){
            return mMaxItemsPerRow;
        }

        return remainingItems;
    }

    private int getClientAreaRowCount(int applicationCount){

        int launchRowCount = applicationCount / mMaxItemsPerRow;

        if (applicationCount % mMaxItemsPerRow != 0){
            launchRowCount++;
        }

        return launchRowCount;
    }

    private LinearLayout getClientAreaRowLinearLayout(){

        LinearLayout linearLayout = new LinearLayout(App.getContext());
        linearLayout.setLayoutParams(getLinearLayoutLayoutParams());
        linearLayout.setOrientation(LinearLayout.HORIZONTAL);
        //linearLayout.setBackgroundColor(Color.RED);

        return linearLayout;
    }

    private Bitmap getBitmapFromApplication(String packageName, String resouceName){

        Resources res = Utilities.getResourceFromApplication(App.getContext(), packageName);
        int id = res.getIdentifier(resouceName, "drawable", packageName);
        return BitmapFactory.decodeResource(res, id);
    }

    private View.OnClickListener clickListener = new View.OnClickListener() {
        @Override
        public void onClick(View view) {

         launchApplication((ActivityLaunchInfoModel)view.getTag());
        }
    };

    private void launchApplication(ActivityLaunchInfoModel activityLauncher){

        try {
            if (mUser == null){
                MessageManager.showMessage(mActivity.getResources().getString(R.string.please_restart_the_application), ErrorSeverity.None);
                return;
            }

            if (Utilities.validateUserAccess(mUser, activityLauncher.getPackageName()) == false){
                return;
            }

            Intent launchIntent = new Intent();
            launchIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            launchIntent.putExtra(Constants.USER, mUser);
            launchIntent.putExtra(Constants.DISTRICT, mDistrict);
            launchIntent.putExtra(Constants.MOBILE_DEVICE, mMobileDevice);
            launchIntent.putExtra(Constants.PAYMENT_CONTEXT, PaymentContext.Unknown);
            launchIntent.putExtra(Constants.SEARCH_FINES_CRITERIA, SearchFinesCriteriaType.Unknown);
            launchIntent.putExtra(Constants.CORE_END_POINT, EndPointConfigModel.getInstance().getCoreGateway());
            launchIntent.putExtra(Constants.ITS_END_POINT, EndPointConfigModel.getInstance().getITSGateway());
            launchIntent.putExtra(Constants.EVR_END_POINT, EndPointConfigModel.getInstance().getEVRGateway());
            launchIntent.putExtra(Constants.PRINTER_MAC_ADDRESS, printerMacAddress());

            launchIntent.setClassName(activityLauncher.getPackageName(), activityLauncher.getClassName());

            if (launchIntent != null) {
                mActivity.startActivityForResult(launchIntent, activityLauncher.getRequestCode());
            }else{
                MessageManager.showMessage("MainActivity::launchApplication() Package name not found", ErrorSeverity.High);
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "MainActivity::launchApplication()"), ErrorSeverity.High);
        };
    }

    public String printerMacAddress(){

        String[] details = Utilities.getPrinterDetails();
        if (details.length != 2){

            MessageManager.showMessage(mActivity.getResources().getString(R.string.printer_details_not_found_please_register_a_printer), ErrorSeverity.None);
            return null;
        }

        return details[1];
    }

    private ImageButton getImageButton(Bitmap bitmap, String packageName, int activityRequestCode, int rightMargin){

        ImageButton imageButton = new ImageButton(App.getContext());
        imageButton.setLayoutParams(getImageButtonLayoutParams(rightMargin));
        imageButton.setScaleType(ImageView.ScaleType.FIT_XY);
        imageButton.setBackgroundColor(Color.TRANSPARENT);
        imageButton.setOnClickListener(clickListener);
        imageButton.setImageBitmap(bitmap);
        imageButton.setTag(getActivityLaunchInfo(packageName, activityRequestCode));

        return imageButton;
    }

    private ImageView getImageView(Bitmap bitmap, String packageName, int activityRequestCode, int rightMargin){

        ImageView imageView = new ImageButton(App.getContext());
        imageView.setLayoutParams(getImageButtonLayoutParams(rightMargin));
        imageView.setScaleType(ImageView.ScaleType.FIT_XY);
        imageView.setBackgroundColor(Color.TRANSPARENT);
        imageView.setOnClickListener(clickListener);
        imageView.setImageBitmap(bitmap);
        imageView.setTag(getActivityLaunchInfo(packageName, activityRequestCode));

        return imageView;
    }

    private float perspectiveCorrectionFactor(int itemSize){

        float sizeDifferenceFactor = 0;

        if (itemSize > MIN_ITEM_SIZE) {
            sizeDifferenceFactor = (float) itemSize / (float) MIN_ITEM_SIZE / 10;
        }

        return 1.4f - sizeDifferenceFactor;
    }

    private LinearLayout.LayoutParams getLinearLayoutLayoutParams(){

        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        layoutParams.gravity = Gravity.CENTER;

        return layoutParams;
    }

    private LinearLayout.LayoutParams getImageButtonLayoutParams(int rightMargin){

        int width = ITEM_SIZE;
        float height = (float) (ITEM_SIZE * perspectiveCorrectionFactor(ITEM_SIZE));//ITEM_HEIGHT_SIZING_FACTOR);
        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(width, (int) height);
        layoutParams.setMargins(0, 0, rightMargin, 0);//(int left, int top, int right, int bottom)
        layoutParams.gravity = Gravity.CENTER;

        return layoutParams;
    }

    private ActivityLaunchInfoModel getActivityLaunchInfo(String packageName, int requestCode){

        ActivityLaunchInfoModel activityLaunchInfo = new ActivityLaunchInfoModel();
        //Request code is configItemID
        activityLaunchInfo.setRequestCode(requestCode);
        activityLaunchInfo.setPackageName(packageName);
        activityLaunchInfo.setClassName(String.format("%s.%s", packageName, "MainActivity"));

        return activityLaunchInfo;
    }
}
