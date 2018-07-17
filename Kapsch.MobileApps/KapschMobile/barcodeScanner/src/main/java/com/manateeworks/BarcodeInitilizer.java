package com.manateeworks;

import android.app.Activity;
import android.graphics.Rect;
import android.view.View;
import android.widget.ImageView;

import za.co.tmt.iticket.ErrorManager;
import za.co.tmt.iticket.ErrorSeverity;


/**
 * Created by CSenekal on 2017/03/21.
 */
public class BarcodeInitilizer {

    public static final boolean PDF_OPTIMIZED = false;
    public static final Rect RECT_LANDSCAPE_1D = new Rect(3, 20, 94, 60);
    public static final Rect RECT_FULL_1D = new Rect(3, 3, 94, 94);
    public static final Rect RECT_FULL_2D = new Rect(20, 5, 60, 90);
    public static final Rect RECT_DOTCODE = new Rect(30, 20, 40, 60);
    public static final int USE_RESULT_TYPE = BarcodeScanner.MWB_RESULT_TYPE_MW;

    public static void InitBarcodeScanning(Activity activity){

        // register your copy of library with given key
        int registerResult = BarcodeScanner.MWBregisterSDK("8LcgVi9LhoZtbdSZ1A8PVCHBpSe7KWD79djxFuOwWN8=", activity);

        switch (registerResult) {
            case BarcodeScanner.MWB_RTREG_OK:
                break;
            case BarcodeScanner.MWB_RTREG_INVALID_KEY:
                ErrorManager.showMessage("MWBregisterSDK() Registration Invalid Key", ErrorSeverity.Low);
                break;
            case BarcodeScanner.MWB_RTREG_INVALID_CHECKSUM:
                ErrorManager.showMessage("MWBregisterSDK() Registration Invalid Checksum", ErrorSeverity.Low);
                break;
            case BarcodeScanner.MWB_RTREG_INVALID_APPLICATION:
                ErrorManager.showMessage("MWBregisterSDK() Registration Invalid Application", ErrorSeverity.Low);
                break;
            case BarcodeScanner.MWB_RTREG_INVALID_SDK_VERSION:
                ErrorManager.showMessage("MWBregisterSDK() Registration Invalid SDK Version", ErrorSeverity.Low);
                break;
            case BarcodeScanner.MWB_RTREG_INVALID_KEY_VERSION:
                ErrorManager.showMessage("MWBregisterSDK() Registration Invalid Key Version", ErrorSeverity.Low);
                break;
            case BarcodeScanner.MWB_RTREG_INVALID_PLATFORM:
                ErrorManager.showMessage("MWBregisterSDK() Registration Invalid Platform", ErrorSeverity.Low);
                break;
            case BarcodeScanner.MWB_RTREG_KEY_EXPIRED:
                ErrorManager.showMessage("MWBregisterSDK() Registration Key Expired", ErrorSeverity.Low);
                break;
            default:
                ErrorManager.showMessage("MWBregisterSDK() Registration Unknown Error", ErrorSeverity.Low);
                break;
        }
        // choose code type or types you want to search for

        if (PDF_OPTIMIZED) {
            BarcodeScanner.MWBsetDirection(BarcodeScanner.MWB_SCANDIRECTION_HORIZONTAL);
            BarcodeScanner.MWBsetActiveCodes(BarcodeScanner.MWB_CODE_MASK_PDF);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_PDF,	RECT_LANDSCAPE_1D);
        } else {
            // Our sample app is configured by default to search both
            // directions...
            BarcodeScanner.MWBsetDirection(BarcodeScanner.MWB_SCANDIRECTION_HORIZONTAL | BarcodeScanner.MWB_SCANDIRECTION_VERTICAL);
            // Our sample app is configured by default to search all supported
            // barcodes...
            BarcodeScanner.MWBsetActiveCodes(
                    BarcodeScanner.MWB_CODE_MASK_25
                            | BarcodeScanner.MWB_CODE_MASK_39
                            | BarcodeScanner.MWB_CODE_MASK_93
                            | BarcodeScanner.MWB_CODE_MASK_128
                            | BarcodeScanner.MWB_CODE_MASK_AZTEC
                            | BarcodeScanner.MWB_CODE_MASK_DM
                            | BarcodeScanner.MWB_CODE_MASK_EANUPC
                            | BarcodeScanner.MWB_CODE_MASK_PDF
                            | BarcodeScanner.MWB_CODE_MASK_QR
                            | BarcodeScanner.MWB_CODE_MASK_CODABAR
                            | BarcodeScanner.MWB_CODE_MASK_11
                            | BarcodeScanner.MWB_CODE_MASK_MSI
                            | BarcodeScanner.MWB_CODE_MASK_RSS);

            // set the scanning rectangle based on scan direction(format in pct:
            // x, y, width, height)
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_25,RECT_FULL_1D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_39,RECT_FULL_1D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_93,RECT_FULL_1D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_128,	RECT_FULL_1D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_AZTEC, RECT_FULL_2D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_DM,RECT_FULL_2D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_EANUPC, RECT_FULL_1D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_PDF,RECT_FULL_1D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_QR,RECT_FULL_2D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_RSS,RECT_FULL_1D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_CODABAR, RECT_FULL_1D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_DOTCODE, RECT_DOTCODE);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_11,RECT_FULL_1D);
            BarcodeScanner.MWBsetScanningRect(BarcodeScanner.MWB_CODE_MASK_MSI,RECT_FULL_1D);
        }

        // set decoder effort level (1 - 5)
        // for live scanning scenarios, a setting between 1 to 3 will suffice
        // levels 4 and 5 are typically reserved for batch scanning
        BarcodeScanner.MWBsetLevel(2);
        BarcodeScanner.MWBsetResultType(USE_RESULT_TYPE);

        // Set minimum result length for low-protected barcode types
        BarcodeScanner.MWBsetMinLength(BarcodeScanner.MWB_CODE_MASK_25, 5);
        BarcodeScanner.MWBsetMinLength(BarcodeScanner.MWB_CODE_MASK_MSI, 5);
        BarcodeScanner.MWBsetMinLength(BarcodeScanner.MWB_CODE_MASK_39, 5);
        BarcodeScanner.MWBsetMinLength(BarcodeScanner.MWB_CODE_MASK_CODABAR, 5);
        BarcodeScanner.MWBsetMinLength(BarcodeScanner.MWB_CODE_MASK_11, 5);
    }
}
