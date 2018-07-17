package za.co.kapsch.ivehicletest;

import android.app.Activity;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.net.Uri;
import android.os.Bundle;
import android.os.Message;
import android.provider.MediaStore;
import android.text.Editable;
import android.text.InputType;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.text.method.HideReturnsTransformationMethod;
import android.text.method.PasswordTransformationMethod;
import android.view.ActionMode;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.TextView;

import org.w3c.dom.Text;

import java.sql.SQLException;
import java.util.Date;
import java.util.List;

import za.co.kapsch.ivehicletest.Enums.InspectionEvidenceType;
import za.co.kapsch.ivehicletest.Enums.QuestionType;
import za.co.kapsch.ivehicletest.General.Constants;
import za.co.kapsch.ivehicletest.General.DataSynchronisation;
import za.co.kapsch.ivehicletest.Models.EvidenceModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionAnswerModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionQuestionModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultsModel;
import za.co.kapsch.ivehicletest.Printer.InspectionTerminateSlip;
import za.co.kapsch.ivehicletest.orm.EvidenceRepository;
import za.co.kapsch.ivehicletest.orm.VehicleInspectionResultsRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Interfaces.IMessageCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;

import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;

public class VehicleInspectionFragment extends WizardPage implements IAsyncProcessCallBack, IMessageCallBack {

    private VehicleInspectionAnswerModel mVehicleInspectionAnswer;
    private RadioGroup mRadioGroup;
    private TextView mQuestionTextView;
    private EditText mAnswerEditText;
    private TextView mAnswerTextView;
    private EditText mConfirmAnswerEditText;
    private EditText mCommentEditText;
    private LinearLayout mCommentLinearLayout;
    private ImageButton mLastWizardpageImageButton;
    private VehicleInspectionQuestionModel mVehicleInspectionQuestion;

    private LinearLayout mAnswerLinearLayout;
    private LinearLayout mInnerAnswerLinearLayout;
    private LinearLayout mConfirmAnswerLinearLayout;
    private LinearLayout mCommentValidationImageButtonLinearLayout;
    private LinearLayout mAnswerValidationImageButtonLinearLayout;
    private LinearLayout mConfirmAnswerValidationImageButtonLinearLayout;
    private ImageButton mCommentValidationImageButton;
    private ImageButton mAnswerValidationImageButton;
    private ImageButton mConfirmAnswerValidationImageButton;
    private LinearLayout mPhotoLinearLayout;
    private LinearLayout mQuestionLinearLayout;
    private ListView mEvidenceListView;
    private ImageButton mCapturePhotoImageButton;
    private EvidenceModel mEvidence;

    //This is a hard coded id value and has to equal to the ID of multiple choice change value in the database.
    public static final int MULTIPLE_CHOICE_CHANGE_ID = 3;

    private TextWatcher textWatcher = new TextWatcher() {
        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {

        }

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

        }

        @Override
        public void afterTextChanged(Editable s) {
            validateUserInterface(false);
        }
    };

    private RadioGroup.OnCheckedChangeListener radioListener = new RadioGroup.OnCheckedChangeListener(){
        @Override
        public void onCheckedChanged (RadioGroup group,int checkedId) {
            validateUserInterface(false);
        }
    };

    //this prevents copy/past on EditText
    private ActionMode.Callback mCustomSelectionActionCallBack = new ActionMode.Callback() {

        public boolean onPrepareActionMode(ActionMode mode, Menu menu) {
            return false;
        }

        public void onDestroyActionMode(ActionMode mode) {
        }

        public boolean onCreateActionMode(ActionMode mode, Menu menu) {
            return false;
        }

        public boolean onActionItemClicked(ActionMode mode, MenuItem item) {
            return false;
        }
    };

    public VehicleInspectionFragment() {
        // Required empty public constructor
    }

    private View.OnClickListener mOnClickListener = new View.OnClickListener() {
        @Override
        public void onClick(View view) {
            wizardActivity().lastPage();
        }
    };

    private View.OnClickListener mValidationButtonOnClickListener = new View.OnClickListener() {
        @Override
        public void onClick(View view) {

            if (view.getId() == R.id.answerValidationImageButton) {
                if ( ((boolean)mAnswerValidationImageButton.getTag()) == true){
                    return;
                }
            }

            if (view.getId() == R.id.commentValidationImageButton) {
                if ( ((boolean)mCommentValidationImageButton.getTag()) == true){
                    return;
                }
            }

            validateUserInterface(true);
            showValidationMessage(view);
        }
    };

    private View.OnClickListener mCapturePhotoButtonOnClickListener = new View.OnClickListener() {
        @Override
        public void onClick(View view) {

            mEvidence = null;
            capturePhoto(false);
        }
    };

    private DialogInterface.OnClickListener mAlertDialogTerminateWizardOnClickListener = new DialogInterface.OnClickListener() {
        @Override
        public void onClick(DialogInterface dialog, int which) {
            try {
                if (which == Constants.OK) {
                    if (saveVehicleInspectionResult() == true) {

                        String comment = mCommentEditText.getText().toString();

                        if (TextUtils.isEmpty(comment)) {
                            comment = getResources().getString(R.string.value_for_does_not_match_reference_value, mVehicleInspectionQuestion.getTestQuestionDescription());
                        }

                        printTerminationSlip(getResources().getString(R.string.inspection_terminated, comment));
                    }
                }
            }catch (Exception e){
                MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionFragment::onClick()"), ErrorSeverity.Medium);
                wizardActivity().enableNextButton(true);
            }
        }
    };

    private DialogInterface.OnClickListener mAlertDialogSlipOnClickListener = new DialogInterface.OnClickListener() {
        @Override
        public void onClick(DialogInterface dialog, int which) {
            try {
                if (which == Constants.YES) {
                     processUpdates();
                     wizardActivity().finish();
                }else{
                    wizardActivity().enableNextButton(true);
                }
            }catch (Exception e){
                MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionFragment::onClick()"), ErrorSeverity.Medium);
                wizardActivity().enableNextButton(true);
            }
        }
    };

    private View.OnFocusChangeListener mAnswerEditTextOnFocusChangeListener = new View.OnFocusChangeListener() {
        @Override
        public void onFocusChange(View v, boolean hasFocus) {

//            if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice){
//                return;
//            }

            if (hasFocus == false) {
                mAnswerEditText.setInputType(InputType.TYPE_TEXT_VARIATION_PASSWORD | InputType.TYPE_TEXT_FLAG_CAP_CHARACTERS);
                mAnswerEditText.setTransformationMethod(PasswordTransformationMethod.getInstance());
            }else{
                mAnswerEditText.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_FLAG_CAP_CHARACTERS);
                mAnswerEditText.setTransformationMethod(HideReturnsTransformationMethod.getInstance());
                mConfirmAnswerEditText.setText(Constants.EMPTY_STRING);
                //mConfirmAnswerValidationImageButton.setVisibility(View.INVISIBLE);

                //JCS commented out on 03/04/2017
                //Utilities.hideView(mConfirmAnswerValidationImageButtonLinearLayout);
            }
        }
    };

    private View.OnFocusChangeListener mConfirmAnswerEditTextOnFocusChangeListener = new View.OnFocusChangeListener() {
        @Override
        public void onFocusChange(View v, boolean hasFocus) {

//            if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice){
//                return;
//            }

            if (hasFocus == false) {
                mConfirmAnswerEditText.setInputType(InputType.TYPE_TEXT_VARIATION_PASSWORD | InputType.TYPE_TEXT_FLAG_CAP_CHARACTERS);
                mConfirmAnswerEditText.setTransformationMethod(PasswordTransformationMethod.getInstance());
                validateUserInterface(false);
            }else{
                mConfirmAnswerEditText.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_FLAG_CAP_CHARACTERS);
                mConfirmAnswerEditText.setTransformationMethod(HideReturnsTransformationMethod.getInstance());
            }
        }
    };


    private AdapterView.OnItemLongClickListener mOnItemLongClickListener = new AdapterView.OnItemLongClickListener() {

        @Override
        public boolean onItemLongClick(AdapterView<?> parent, View view, int position, long id) {

            mEvidence = (EvidenceModel) mEvidenceListView.getItemAtPosition(position);
            if (mEvidence != null) {
                capturePhoto(true);
            }

            return true;
        }
    };

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        View rootView =  inflater.inflate(R.layout.fragment_vehicle_inspection, container, false);

        mAnswerEditText = (EditText) rootView.findViewById(R.id.answerEditText);
        mAnswerTextView  = (TextView) rootView.findViewById(R.id.answerTextView);
        mQuestionTextView = (TextView) rootView.findViewById(R.id.questionTextView);
        mConfirmAnswerEditText = (EditText) rootView.findViewById(R.id.confirmAnswerEditText);
        mRadioGroup = (RadioGroup) rootView.findViewById(R.id.radioGroup);
        mCommentEditText = (EditText) rootView.findViewById(R.id.commentEditText);
        mCommentLinearLayout  = (LinearLayout) rootView.findViewById(R.id.commentLinearLayout);
        mLastWizardpageImageButton = (ImageButton) rootView.findViewById(R.id.lastWizardPageImageButton);
        mAnswerLinearLayout = (LinearLayout)  rootView.findViewById(R.id.answerLinearLayout);
        mInnerAnswerLinearLayout = (LinearLayout)  rootView.findViewById(R.id.innerAnswerLinearLayout);
        mConfirmAnswerLinearLayout = (LinearLayout)  rootView.findViewById(R.id.confirmLinearLayout);
        mCommentValidationImageButtonLinearLayout = (LinearLayout)  rootView.findViewById(R.id.commentValidationImageButtonLinearLayout);
        mAnswerValidationImageButtonLinearLayout = (LinearLayout)  rootView.findViewById(R.id.answerValidationImageButtonLinearLayout);
        mConfirmAnswerValidationImageButtonLinearLayout  = (LinearLayout)  rootView.findViewById(R.id.confirmAnswerValidationImageButtonLinearLayout);

        mCommentValidationImageButton = (ImageButton)  rootView.findViewById(R.id.commentValidationImageButton);
        mAnswerValidationImageButton = (ImageButton)  rootView.findViewById(R.id.answerValidationImageButton);
        mConfirmAnswerValidationImageButton = (ImageButton)  rootView.findViewById(R.id.confirmAnswerValidationImageButton);
        mPhotoLinearLayout = (LinearLayout)  rootView.findViewById(R.id.photoLinearLayout);
        mQuestionLinearLayout  = (LinearLayout)  rootView.findViewById(R.id.questionLinearLayout);
        mEvidenceListView  = (ListView)  rootView.findViewById(R.id.evidenceListView);
        mCapturePhotoImageButton = (ImageButton)  rootView.findViewById(R.id.capturePhotoImageButton);

        mAnswerTextView.setText(Constants.EMPTY_STRING);
        mCommentValidationImageButton.setOnClickListener(mValidationButtonOnClickListener);
        mAnswerValidationImageButton.setOnClickListener(mValidationButtonOnClickListener);
        mConfirmAnswerValidationImageButton.setOnClickListener(mValidationButtonOnClickListener);
        mCapturePhotoImageButton.setOnClickListener(mCapturePhotoButtonOnClickListener);
        mEvidenceListView.setOnItemLongClickListener(mOnItemLongClickListener);

        Bundle bundle = getArguments();
        mVehicleInspectionQuestion = bundle.getParcelable(Constants.ADDITIONAL_WIZARD_INFORMATION);

        try {
            setupScreen();
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionFragment::onCreateView()"), ErrorSeverity.High);
        }

        mAnswerEditText.setOnFocusChangeListener(mAnswerEditTextOnFocusChangeListener);
        mRadioGroup.setOnCheckedChangeListener(radioListener);
        mAnswerEditText.addTextChangedListener(textWatcher);
        mAnswerEditText.setCustomSelectionActionModeCallback(mCustomSelectionActionCallBack);
        mConfirmAnswerEditText.addTextChangedListener(textWatcher);
        mConfirmAnswerEditText.setOnFocusChangeListener(mConfirmAnswerEditTextOnFocusChangeListener);
        mConfirmAnswerEditText.setCustomSelectionActionModeCallback(mCustomSelectionActionCallBack);
        mCommentEditText.addTextChangedListener(textWatcher);

        mLastWizardpageImageButton.setOnClickListener(mOnClickListener);

//        if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice) {
//            Utilities.hideView(mAnswerValidationImageButtonLinearLayout);
//            Utilities.hideView(mConfirmAnswerValidationImageButtonLinearLayout);
//        }
        //mConfirmAnswerValidationImageButton.setVisibility(View.INVISIBLE);

        return rootView;
    }

    @Override
    public boolean validateNextPage(){

        if (mVehicleInspectionQuestion.getWeight() >= Constants.TERMINATE_INSPECTION_WEIGHT) {

            if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice) {
                if (mVehicleInspectionQuestion.isCorrectQuestionAnswerID(getVehicleInspectionAnswer().getTestQuestionAnswerID()) == false) {
                    Utilities.displayOkMessageEx(
                            getResources().getString(R.string.failing_this_question_will_terminate_the_inspection),
                            this.getActivity(),
                            mAlertDialogTerminateWizardOnClickListener);

                    return false;
                }
            }else if (mAnswerEditText.getText().toString().trim().equals(mVehicleInspectionQuestion.getComparedValue().trim()) == false) {
                Utilities.displayOkMessageEx(
                        getResources().getString(R.string.failing_this_question_will_terminate_the_inspection),
                        this.getActivity(),
                        mAlertDialogTerminateWizardOnClickListener);

                return false;

            }
        }

        if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice){
            //if ((mVehicleInspectionQuestion.getIsDoubleEntry() == true) && (mVehicleInspectionQuestion.getIsCompared() == true)) {
            if (mVehicleInspectionQuestion.getIsCompared() == true) {
                if (mAnswerEditText.getText().toString().trim().equals(mVehicleInspectionQuestion.getComparedValue().trim()) == true) {
                    if (mVehicleInspectionQuestion.getWeight() >= Constants.TERMINATE_INSPECTION_WEIGHT) {
                        if (mVehicleInspectionQuestion.isCorrectQuestionAnswerID(getVehicleInspectionAnswer().getTestQuestionAnswerID()) == false) {
                            return true;
                        }

                        Utilities.displayOkMessage(
                                String.format(
                                        getResources().getString(R.string.captured_value_matches_previous_value),
                                        mVehicleInspectionQuestion.getTestQuestionDescription()),
                                this.getActivity());

                        return false;
                    }
                }
            }
        }

        return true;
    }


//    @Override
//    public boolean validateNextPage(){
//
//        if (mVehicleInspectionQuestion.getWeight() >= Constants.TERMINATE_INSPECTION_WEIGHT) {
//
//            if ((mVehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice)&&
//                (mVehicleInspectionQuestion.isCorrectQuestionAnswerID(getVehicleInspectionAnswer().getTestQuestionAnswerID()) == false)) {
//                    Utilities.displayOkMessageEx(
//                            getResources().getString(R.string.failing_this_question_will_terminate_the_inspection),
//                            this.getActivity(),
//                            mAlertDialogTerminateWizardOnClickListener);
//
//                    return false;
//            }else if (mAnswerEditText.getText().toString().trim().equals(mVehicleInspectionQuestion.getComparedValue().trim()) == false) {
//                Utilities.displayOkMessageEx(
//                        getResources().getString(R.string.failing_this_question_will_terminate_the_inspection),
//                        this.getActivity(),
//                        mAlertDialogTerminateWizardOnClickListener);
//
//                return false;
//            }
//
//            if ((mVehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice)&&
//                (mVehicleInspectionQuestion.getIsCompared() == true)&&
//                (mAnswerEditText.getText().toString().trim().equals(mVehicleInspectionQuestion.getComparedValue().trim()) == true)&&
//                (mVehicleInspectionQuestion.isCorrectQuestionAnswerID(getVehicleInspectionAnswer().getTestQuestionAnswerID()) == true)) {
//
//                    Utilities.displayOkMessage(
//                            String.format(
//                                    getResources().getString(R.string.captured_value_matches_previous_value),
//                                    mVehicleInspectionQuestion.getTestQuestionDescription()),
//                            this.getActivity());
//
//                    return false;
//            }
//        }
//
//        return true;
//    }


    @Override
    public void beforeCancel(){
        try {
            EvidenceRepository.deleteCancelledEvidence();
        }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionFragment::beforeCancel()"), ErrorSeverity.High);
        }
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {
        try {

            if (asyncResultModel == null) {
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_ASYNC_PROCESS_PRINT:

                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            Utilities.displayDecisionMessage(getResources().getString(R.string.did_the_slip_print_successfully), this.getContext(), mAlertDialogSlipOnClickListener);
                            break;
                        case FAILED:
                            break;
                    }

                    break;
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
            return;
        }
    }

    private void setupScreen() throws Exception{

        if (wizardActivity().lastPageReached() == false) {
            Utilities.hideView(mLastWizardpageImageButton);
        }

        mQuestionTextView.setText(mVehicleInspectionQuestion.getTestQuestionDescription());

        if (mVehicleInspectionQuestion.isPhotoRequired() == true){
            photoScreen();
            return;
        }else{
            Utilities.hideView(mPhotoLinearLayout);
        }

        if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.Text) {
            setupScreenForText(mVehicleInspectionQuestion);
        }else if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.MultipleChoice) {
            setupScreenForMultipleChoice();
        }else if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.TextMultipleChoice){
            setupScreenForTextMultipleChoice();
        }else if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice){
            setupScreenForChangeMultipleChoice();
        }

        Utilities.hideView(mAnswerValidationImageButtonLinearLayout);
        Utilities.hideView(mConfirmAnswerValidationImageButtonLinearLayout);
        Utilities.hideView(mCommentValidationImageButtonLinearLayout);

        if ((mVehicleInspectionQuestion.getQuestionType() == QuestionType.Text) && (mVehicleInspectionQuestion.getIsDoubleEntry() == true)) {
            Utilities.showViewWrapContent(mAnswerValidationImageButtonLinearLayout);
            Utilities.showViewWrapContent(mConfirmAnswerValidationImageButtonLinearLayout);
        }

        initialiseEditTextState();
    }

    private void photoScreen(){
        Utilities.hideView(mQuestionLinearLayout);
        mNextPageID = mVehicleInspectionQuestion.getVehicleInspectionAnswerList().get(0).getNextQuestionId();
        PopulateEvidenceListView();
    }

    private void capturePhoto(boolean recapture){
        try {

            List<EvidenceModel>  evidenceList = EvidenceRepository.getEvidence(mVehicleInspectionQuestion.getBookingID(), InspectionEvidenceType.VehiclePhoto);

            if (recapture == false){
                mEvidence = null;
                if (evidenceList.size() >= Constants.MAX_EVIDENCE_IMAGES) {
                    MessageManager.showMessage(getResources().getString(R.string.maximum_of_three_images_can_be_captured), ErrorSeverity.None);
                    return;
                }
            }

            Uri uri = Uri.fromFile(Utilities.getTicketFile(Constants.TEMP_PICTURE_EVIDENCE_FILENAME));
            Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
            intent.putExtra(MediaStore.EXTRA_OUTPUT, uri);
            startActivityForResult(intent, Constants.REQUEST_TAKE_PHOTO);
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionActivity::capturePhoto()"), ErrorSeverity.Medium);
        }
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == Activity.RESULT_OK) {
            if (requestCode == Constants.REQUEST_TAKE_PHOTO) {
                try {
                    byte[] pictureEvidence = Utilities.getFileData(Utilities.getTicketFile(Constants.TEMP_PICTURE_EVIDENCE_FILENAME));

                    Bitmap bitmap = BitmapFactory.decodeByteArray(pictureEvidence, 0, pictureEvidence.length);
                    bitmap = Utilities.getResizedBitmap(bitmap, Constants.EVIDENCE_BITMAP_MAX_SIZE);
                    byte[] pictureEvidenceJpeg = Utilities.bitmapToJPGBytes(bitmap);

                    if (mEvidence == null) {
                        EvidenceModel evidence = EvidenceModel.getEvidence(
                                InspectionEvidenceType.VehiclePhoto,
                                pictureEvidenceJpeg,
                                mVehicleInspectionQuestion.getBookingID(),
                                wizardActivity().getVehicleInspectionQuery().getSiteID());
                        EvidenceRepository.create(evidence);
                    }else{
                        mEvidence.setEvidence(pictureEvidenceJpeg);
                        EvidenceRepository.create(mEvidence);
                    }

                    if (Utilities.getTicketFile(Constants.TEMP_PICTURE_EVIDENCE_FILENAME).delete() == false) {
                        MessageManager.showMessage("Failed to delete photo evidence file", ErrorSeverity.High);
                    }

                    PopulateEvidenceListView();

                } catch (Exception e) {
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionActivity::onActivityResult()"), ErrorSeverity.High);
                }
            }
        }
    }

    private void PopulateEvidenceListView(){

        try {
            List<EvidenceModel> evidenceList = EvidenceRepository.getEvidence(mVehicleInspectionQuestion.getBookingID(), InspectionEvidenceType.VehiclePhoto);

            if (evidenceList.size() < 1) return;

            EvidenceListAdapter arrayAdapter = new EvidenceListAdapter(this.getActivity(), evidenceList);
            mEvidenceListView.setAdapter(arrayAdapter);

            validateUserInterface(false);

        }catch(Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionActivity::PopulateEvidenceListView()"), ErrorSeverity.High);
        }
    }

    private void setupScreenForText(VehicleInspectionQuestionModel vehicleInspectionQuestion) throws Exception {

        List<VehicleInspectionAnswerModel> vehicleInspectionAnswerList = mVehicleInspectionQuestion.getVehicleInspectionAnswerList();
        if (vehicleInspectionAnswerList.size() != 1)
        {
            throw new Exception("VehicleInspectionFragment::setupScreenForText() - More than one VehicleInspectionAnswer in list.");
        }

        if (vehicleInspectionQuestion.getVehicleInspectionAnswerList().size() == 1) {
            mVehicleInspectionAnswer = vehicleInspectionAnswerList.get(0);
            mNextPageID = vehicleInspectionQuestion.getVehicleInspectionAnswerList().get(0).getNextQuestionId();
        }else{
            throw new Exception("VehicleInspectionFragment::setupScreenForText:: vehicleInspectionAnswer is not in valid state");
        }

        if (mVehicleInspectionQuestion.getIsDoubleEntry() == false){
            Utilities.hideView(mConfirmAnswerLinearLayout);
        }

        if (mVehicleInspectionQuestion.getIsReadOnly() == false){
            Utilities.hideView(mAnswerTextView);
        }else{
            mAnswerTextView.setText(mVehicleInspectionQuestion.getComparedValue());
        }

    }

    private void setupScreenForMultipleChoice(){

        for (VehicleInspectionAnswerModel vehicleInspectionAnswer : mVehicleInspectionQuestion.getVehicleInspectionAnswerList()) {
            RadioButton radioButton = addRadioButton(mRadioGroup.getChildCount(), vehicleInspectionAnswer);
            mRadioGroup.addView(radioButton);

            ViewGroup.LayoutParams layoutParams = radioButton.getLayoutParams();
            layoutParams.height = 100;
            radioButton.setLayoutParams(layoutParams);
        }

        Utilities.hideView(mAnswerEditText);
        Utilities.hideView(mAnswerLinearLayout);
        Utilities.hideView(mInnerAnswerLinearLayout);
    }

    private void setupScreenForTextMultipleChoice(){

        for (VehicleInspectionAnswerModel vehicleInspectionAnswer : mVehicleInspectionQuestion.getVehicleInspectionAnswerList()) {
            RadioButton radioButton = addRadioButton(mRadioGroup.getChildCount(), vehicleInspectionAnswer);
            mRadioGroup.addView(radioButton);

            ViewGroup.LayoutParams layoutParams = radioButton.getLayoutParams();
            layoutParams.height = 100;
            radioButton.setLayoutParams(layoutParams);
        }

        mAnswerTextView.setText(mVehicleInspectionQuestion.getComparedValue());

        Utilities.hideView(mInnerAnswerLinearLayout);
        Utilities.hideView(mConfirmAnswerLinearLayout);
    }

    private void setupScreenForChangeMultipleChoice(){

        for (VehicleInspectionAnswerModel vehicleInspectionAnswer : mVehicleInspectionQuestion.getVehicleInspectionAnswerList()) {
            RadioButton radioButton = addRadioButton(mRadioGroup.getChildCount(), vehicleInspectionAnswer);
            mRadioGroup.addView(radioButton);

            ViewGroup.LayoutParams layoutParams = radioButton.getLayoutParams();
            layoutParams.height = 100;
            radioButton.setLayoutParams(layoutParams);
        }

        if(mVehicleInspectionQuestion.getIsReadOnly() == true) {
            mAnswerTextView.setText(mVehicleInspectionQuestion.getComparedValue());
        }

        if (mVehicleInspectionQuestion.getIsDoubleEntry() == false){
            Utilities.hideView(mConfirmAnswerLinearLayout);
        }
    }

    private RadioButton addRadioButton(int id, VehicleInspectionAnswerModel vehicleInspectionAnswerModel) {

        RadioButton radioButton = new RadioButton(this.getContext());
        radioButton.setId(id);
        radioButton.setTag(vehicleInspectionAnswerModel);
        radioButton.setTextAppearance(getContext(), android.R.style.TextAppearance_Medium);
        radioButton.setText(vehicleInspectionAnswerModel.getQuestionAnswerDescription());
        return radioButton;
    }

    private boolean validateValuePattern(){

        boolean result = true;

        if (TextUtils.isEmpty(mVehicleInspectionQuestion.getValuePattern()) == false) {

            if (TextUtils.isEmpty(mAnswerEditText.getText().toString().trim()) == true){
                return false;
            }

            if (TextUtils.isEmpty(mAnswerEditText.getText().toString().trim()) == false) {
                List<String> regExResult = Utilities.getRegexMatches(mAnswerEditText.getText().toString(), mVehicleInspectionQuestion.getValuePattern());
                result = (regExResult.size() > 0);
            }
        }

        return result;
    }

    private void validateUserInterface(boolean showMessage){

        if (validatePhoto() == true){
            return;
        }

        switch (mVehicleInspectionQuestion.getQuestionType()){
            case Text: validateText(showMessage);
                return;
            case MultipleChoice:validateMultipleChoice(showMessage);
                return;
            case TextMultipleChoice: validateMultipleChoice(showMessage);
                return;
            case ChangeMultipleChoice: validateChangeMultipleChoice(showMessage);
                return;
        }
    }

    private void showValidationMessage(View view){

//        VehicleInspectionAnswerModel vehicleInspectionAnswer = validateChangeMutipleChoice();
//        if (vehicleInspectionAnswer.getIsCommentRequired()){
//
//        }

        if (view.getId() != R.id.commentValidationImageButton) {

            boolean valuePatternValidated = validateValuePattern();
            if (valuePatternValidated == false) {
                MessageManager.showMessage(getResources().getString(R.string.is_not_a_valid_value), ErrorSeverity.None);
                return;
            }

            boolean valueRequired = TextUtils.isEmpty(mAnswerEditText.getText().toString()) == false;
            if (valueRequired == false) {
                MessageManager.showMessage(getResources().getString(R.string.value_required), ErrorSeverity.None);
                return;
            }

            if (mVehicleInspectionQuestion.getIsDoubleEntry() == true) {
                boolean valuesMatch = validateConfirmAnswerMatchesAnswer();
                if (valuesMatch == false) {
                    MessageManager.showMessage(getResources().getString(R.string.double_value_entries_do_not_match), ErrorSeverity.None);
                    return;
                }
            }
        }

        boolean mutipleChoice = validateMutipleChoice();
        if (mutipleChoice == false) {
            MessageManager.showMessage(getResources().getString(R.string.comment_required), ErrorSeverity.None);
            return;
        }
    }

    private boolean validatePhoto(){

        try {
            if (mVehicleInspectionQuestion.isPhotoRequired() == true) {
                Utilities.hideView(mQuestionLinearLayout);
                if (EvidenceRepository.getEvidence(mVehicleInspectionQuestion.getBookingID(), InspectionEvidenceType.VehiclePhoto).size() > 0) {
                    wizardActivity().enableNextButton(true);
                }else{
                    wizardActivity().enableNextButton(false);
                }
                return true;
            }
        }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionFragment::validatePhoto()"), ErrorSeverity.High);
        }

        return false;
    }

    private void validateText(boolean showMessage){

        boolean result;
        boolean valuePatternValidated = (validateValuePattern(showMessage));

        if (mVehicleInspectionQuestion.getIsDoubleEntry() == true){
            result = validateConfirmAnswerMatchesAnswer();
            wizardActivity().enableNextButton(result);
            if (result == false){
                manageValidationMessageButtons(valuePatternValidated, false, false, (TextUtils.isEmpty(mAnswerEditText.getText().toString()) == false));
            }else {
                manageValidationMessageButtons(valuePatternValidated, true, false, (TextUtils.isEmpty(mAnswerEditText.getText().toString()) == false));
            }
        }else{
            wizardActivity().enableNextButton(TextUtils.isEmpty(mAnswerEditText.getText().toString().trim()) == false);
        }
    }

    private void validateMultipleChoice(boolean showMessage){

       boolean result = validateMutipleChoice();
        wizardActivity().enableNextButton(result);
        if (result == false) {
             manageValidationMessageButtons(false, false, false, false);
            return;
        }

        manageValidationMessageButtons(false, true, true, false);
    }

    private void validateChangeMultipleChoice(boolean showMessage){

        VehicleInspectionAnswerModel vehicleInspectionAnswer = validateChangeMutipleChoice();

        if (vehicleInspectionAnswer == null){
            wizardActivity().enableNextButton(false);
            return;
        }

        if (vehicleInspectionAnswer.getIsCommentRequired() == true) {
            Utilities.showViewWrapContent(mCommentValidationImageButtonLinearLayout);
        }else{
            Utilities.hideView(mCommentValidationImageButtonLinearLayout);
        }

        if (vehicleInspectionAnswer.getTestQuestionAnswerID() == MULTIPLE_CHOICE_CHANGE_ID){
            Utilities.showViewWrapContent(mAnswerValidationImageButtonLinearLayout);
            Utilities.showViewWrapContent(mConfirmAnswerValidationImageButtonLinearLayout);

            mAnswerEditText.setEnabled(true);
            mConfirmAnswerEditText.setEnabled(true);

            boolean answerValidated = (TextUtils.isEmpty(mAnswerEditText.getText().toString().trim()) == false);
            manageValidationMessageButtons(false, validateConfirmAnswerMatchesAnswer(), false, answerValidated);
            wizardActivity().enableNextButton(answerValidated && validateConfirmAnswerMatchesAnswer());
            return;
        }

        if (vehicleInspectionAnswer.getTestQuestionAnswerID() != MULTIPLE_CHOICE_CHANGE_ID){
            Utilities.hideView(mAnswerValidationImageButtonLinearLayout);
            Utilities.hideView(mConfirmAnswerValidationImageButtonLinearLayout);

            if (TextUtils.isEmpty(mAnswerEditText.getText().toString().trim()) == false) {
                mAnswerEditText.setText(Constants.EMPTY_STRING);
                mConfirmAnswerEditText.setText(Constants.EMPTY_STRING);
            }
            mAnswerEditText.setEnabled(false);
            mConfirmAnswerEditText.setEnabled(false);

            boolean commentValidated = (TextUtils.isEmpty(mCommentEditText.getText().toString()) == false);
            manageValidationMessageButtons(false, true, commentValidated, false);
            wizardActivity().enableNextButton(commentValidated || (vehicleInspectionAnswer.getIsCommentRequired() == false));
            return;
        }
    }

    private boolean validateValuePattern(boolean showMessage){

        boolean valuePatternValidated = validateValuePattern();
        boolean result = valuePatternValidated;
        if (result == false) {
//            if (showMessage == true) {
//                MessageManager.showMessage(
//                        String.format("%s %s",
//                                mAnswerEditText.getText().toString(),
//                                getResources().getString(R.string.is_not_a_valid_value)),
//                        ErrorSeverity.None);
//            }
            manageValidationMessageButtons(valuePatternValidated, false, false, false);
            mConfirmAnswerEditText.setEnabled(false);
            wizardActivity().enableNextButton(result);
            return false;
        }else{
            manageValidationMessageButtons(valuePatternValidated, false, false, false);
            mConfirmAnswerEditText.setEnabled(true);
            return true;
        }
    }

    private boolean hasValuePattern(){
        return TextUtils.isEmpty(mVehicleInspectionQuestion.getValuePattern()) == false;
    }

    private void manageValidationMessageButtons(boolean valuePatternValidated, boolean valueMatchedValidated, boolean commentValidated, boolean answerValidated){

        if (hasValuePattern() == true) {
            patternMessageButtons(valuePatternValidated);
        }

        if (mVehicleInspectionQuestion.getIsDoubleEntry() == true) {
            matchMessageButtons(hasValuePattern(), valueMatchedValidated);
        }

        if (commentValidated == true){
            mCommentValidationImageButton.setTag(true);
            mCommentValidationImageButton.setImageResource(R.drawable.check);
        }else {
            mCommentValidationImageButton.setTag(false);
            mCommentValidationImageButton.setImageResource(R.drawable.exclamation);
        }

        if ((mVehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice) ||
            (mVehicleInspectionQuestion.getQuestionType() == QuestionType.Text)){
            if (answerValidated == true) {
                mAnswerValidationImageButton.setTag(true);
                mAnswerValidationImageButton.setImageResource(R.drawable.check);
            } else {
                mAnswerValidationImageButton.setTag(false);
                mAnswerValidationImageButton.setImageResource(R.drawable.exclamation);
            }
        }
    }

    private void patternMessageButtons(boolean valuePatternValidated){

        //mConfirmAnswerValidationImageButton.setVisibility(View.INVISIBLE);
        Utilities.hideView(mConfirmAnswerValidationImageButtonLinearLayout);
        if (valuePatternValidated == true) {
            mAnswerValidationImageButton.setTag(true);
            mAnswerValidationImageButton.setImageResource(R.drawable.check);
        } else {
            mAnswerValidationImageButton.setTag(false);
            Utilities.showViewWrapContent(mAnswerValidationImageButtonLinearLayout);
            mAnswerValidationImageButton.setImageResource(R.drawable.exclamation);
        }
    }

    private void matchMessageButtons(boolean valuePatternApplicable, boolean valueMatchedValidated){

        if (valuePatternApplicable == true) {
            if (TextUtils.isEmpty(mConfirmAnswerEditText.getText().toString().trim()) == false) {
                if (TextUtils.isEmpty(mConfirmAnswerEditText.getText().toString().trim()) == false) {
                    //Utilities.showViewWrapContent(mConfirmAnswerValidationImageButtonLinearLayout);
                    if (valueMatchedValidated == true) {
                        mConfirmAnswerValidationImageButton.setImageResource(R.drawable.check);
                    } else {
                        mConfirmAnswerValidationImageButton.setImageResource(R.drawable.exclamation);
                    }
                }
            }
        }else{
            //if (TextUtils.isEmpty(mConfirmAnswerEditText.getText().toString().trim()) == false) {
                //Utilities.showViewWrapContent(mConfirmAnswerValidationImageButtonLinearLayout);
                if (valueMatchedValidated == true) {
                    mConfirmAnswerValidationImageButton.setImageResource(R.drawable.check);
                } else {
                    mConfirmAnswerValidationImageButton.setImageResource(R.drawable.exclamation);
            //    }
            }
        }
    }

    private boolean validateConfirmAnswerMatchesAnswer(){

        if (mVehicleInspectionQuestion.getIsDoubleEntry() == false){
            return true;
        }

        if (TextUtils.isEmpty(mAnswerEditText.getText().toString().trim()) == true)
            return false;

        return (mAnswerEditText.getText().toString().trim().equals(mConfirmAnswerEditText.getText().toString().trim()));
    }

    private boolean validateMutipleChoice(){

        RadioButton radioButton;
        int childCount = mRadioGroup.getChildCount();
        for(int index = 0; index < childCount; index++) {
            radioButton = (RadioButton) mRadioGroup.getChildAt(index);
            if (radioButton.isChecked() == true) {
                VehicleInspectionAnswerModel vehicleInspectionAnswer = (VehicleInspectionAnswerModel)radioButton.getTag();
                mNextPageID = vehicleInspectionAnswer.getNextQuestionId();
                if (vehicleInspectionAnswer.getIsCommentRequired() == true) {
                    Utilities.showViewWrapContent(mCommentValidationImageButtonLinearLayout);
                    return TextUtils.isEmpty(mCommentEditText.getText().toString()) == false;
                }
                Utilities.hideView(mCommentValidationImageButtonLinearLayout);
                return true;
            }
        }
        return false;
    }

    private VehicleInspectionAnswerModel validateChangeMutipleChoice(){

        RadioButton radioButton;
        int childCount = mRadioGroup.getChildCount();

        for(int index = 0; index < childCount; index++) {

            radioButton = (RadioButton) mRadioGroup.getChildAt(index);
            if (radioButton.isChecked() == true) {

                VehicleInspectionAnswerModel vehicleInspectionAnswer = (VehicleInspectionAnswerModel)radioButton.getTag();
                mNextPageID = vehicleInspectionAnswer.getNextQuestionId();
                return vehicleInspectionAnswer;
            }
        }

        return null;
    }


    private String getMutipleChoiceText(){

        RadioButton radioButton;
        int childCount = mRadioGroup.getChildCount();

        for(int index = 0; index < childCount; index++) {

            radioButton = (RadioButton) mRadioGroup.getChildAt(index);
            if (radioButton.isChecked() == true) {
                return radioButton.getText().toString();
            }
        }

        return null;
    }

    private void initialiseEditTextState(){
        if (mVehicleInspectionQuestion.getIsDoubleEntry() == true){
            mAnswerEditText.requestFocus();
            mAnswerEditText.setSelection(mAnswerEditText.getText().length());
            mAnswerEditText.setInputType(InputType.TYPE_TEXT_FLAG_CAP_CHARACTERS);
            //if (TextUtils.isEmpty(mAnswerEditText.getText().toString().trim()) == false) {
                mConfirmAnswerEditText.setInputType(InputType.TYPE_TEXT_FLAG_CAP_CHARACTERS);
                //mConfirmAnswerEditText.setTransformationMethod(PasswordTransformationMethod.getInstance());
            //}
        }
    }

    @Override
    public void onStart()
    {
        super.onStart();

        validateUserInterface(false);

        write();
    }

    @Override
    public void onStop(){

        super.onStop();
        read();
    }

    public VehicleInspectionQuestionModel getVehicleInspectionQuestion(){
        return mVehicleInspectionQuestion;
    }

    private void write(){
        //TODO Check if this is going to be necessary.
    }

    private void read(){

        if (mVehicleInspectionQuestion.isPhotoRequired() == true) return;

        VehicleInspectionAnswerModel vehicleInspectionAnswer = getVehicleInspectionAnswer();

        //if vehicleInspectionAnswer is null then we do not worry to add anything to the result list
        if (vehicleInspectionAnswer == null) return;

        VehicleInspectionResultModel vehicleInspectionResult = new VehicleInspectionResultModel();
        vehicleInspectionResult.setBookingID(wizardActivity().getVehicleInspectionQuery().getBookingID());
        vehicleInspectionResult.setTestTypeID(wizardActivity().getVehicleInspectionQuery().getTestTypeID());
        vehicleInspectionResult.setTestQuestionID(mVehicleInspectionQuestion.getTestQuestionID());

        vehicleInspectionResult.setAnswer(mAnswerEditText.getText().toString().trim()); //multiple choice with compare

        vehicleInspectionResult.setQuestion(mQuestionTextView.getText().toString());
        vehicleInspectionResult.setCompareValue(mVehicleInspectionQuestion.getComparedValue());

        if (vehicleInspectionAnswer != null) {
            vehicleInspectionResult.setTestQuestionAnswerID(vehicleInspectionAnswer.getTestQuestionAnswerID() == 0 ? null : vehicleInspectionAnswer.getTestQuestionAnswerID());
            vehicleInspectionResult.setRelationshipID(vehicleInspectionAnswer.getRelationshipID());
            vehicleInspectionResult.setMultipleChoiceAnswer(TextUtils.isEmpty(vehicleInspectionAnswer.getQuestionAnswerDescription()) ? null : vehicleInspectionAnswer.getQuestionAnswerDescription());
            vehicleInspectionResult.setDisplayColour(getDisplayColour(vehicleInspectionAnswer.getDisplayColour()));
        }

        vehicleInspectionResult.setComments(mCommentEditText.getText().toString());

        List<VehicleInspectionResultModel> vehicleInspectionResultList = wizardActivity().getVehicleInspectionResultList();

        for (int index = 0; index < vehicleInspectionResultList.size(); index++) {

            if (vehicleInspectionResult.getTestQuestionID() == vehicleInspectionResultList.get(index).getTestQuestionID()){
                long id = vehicleInspectionResultList.get(index).getID();
                vehicleInspectionResult.setID(id);
                vehicleInspectionResultList.set(index, vehicleInspectionResult);
                //return;
            }
        }

        if (mVehicleInspectionQuestion.getIsCompared() == true){
            if (mAnswerEditText.getText().toString().trim().equals(mVehicleInspectionQuestion.getComparedValue().trim()) == true){
                vehicleInspectionResult.setIsPassed(1);
                vehicleInspectionResult.setPassFailedText(getResources().getString(R.string.pass));
            }else{
                vehicleInspectionResult.setIsPassed(0);
                vehicleInspectionResult.setPassFailedText(getResources().getString(R.string.fail));
            }
        }

        if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.MultipleChoice ||
            mVehicleInspectionQuestion.getQuestionType() == QuestionType.TextMultipleChoice ||
            mVehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice) {

            if (vehicleInspectionResult.getTestQuestionAnswerID() != null) {
                if (mVehicleInspectionQuestion.isCorrectQuestionAnswerID(vehicleInspectionResult.getTestQuestionAnswerID())) {
                    vehicleInspectionResult.setIsPassed(1);
                    vehicleInspectionResult.setPassFailedText(getResources().getString(R.string.pass));
                } else {
                    vehicleInspectionResult.setIsPassed(0);
                    vehicleInspectionResult.setPassFailedText(getResources().getString(R.string.fail));
                }
            }

            vehicleInspectionResult.setPassFailedText(getMutipleChoiceText());

        }else if (mVehicleInspectionQuestion.getQuestionType() == QuestionType.TextMultipleChoice){
            if (mVehicleInspectionQuestion.isCorrectQuestionAnswerID(vehicleInspectionResult.getTestQuestionAnswerID())) {
                //vehicleInspectionResult.setIsPassed(1);
                vehicleInspectionResult.setPassFailedText(getResources().getString(R.string.pass));
            } else {
                //vehicleInspectionResult.setIsPassed(0);
                vehicleInspectionResult.setPassFailedText(getResources().getString(R.string.fail));
            }
        }

        vehicleInspectionResult.setWeight(mVehicleInspectionQuestion.getWeight());
        vehicleInspectionResult.setQuestionTypeID(mVehicleInspectionQuestion.getQuestionTypeID());

        if (vehicleInspectionResult.getDisplayColour() == Color.BLACK){
            vehicleInspectionResult.setDisplayColour(vehicleInspectionResult.getIsPassed() == 1 ? Color.BLACK : Color.RED);
        }

        for(int index = 0; index < vehicleInspectionResultList.size(); index++) {
            VehicleInspectionResultModel temp = vehicleInspectionResultList.get(index);
            if (temp.getTestQuestionID() == vehicleInspectionResult.getTestQuestionID()){
                vehicleInspectionResultList.remove(temp);
                vehicleInspectionResultList.add(index, vehicleInspectionResult);
                return;
            }
        }

        vehicleInspectionResultList.add(vehicleInspectionResult);
    }

    private int getDisplayColour(String colour) {

        if (TextUtils.isEmpty(colour) == true) {
            return Color.BLACK;
        }

        switch(colour){
            case "RED" : return Color.RED;
            default : return Color.BLACK;
        }
    }

    private VehicleInspectionAnswerModel getVehicleInspectionAnswer(){

        for(int index = 0; index < mRadioGroup.getChildCount(); index++){
            View view  = mRadioGroup.getChildAt(index);
            if (view instanceof RadioButton){
                if (((RadioButton)view).isChecked() == true){
                    return (VehicleInspectionAnswerModel)view.getTag();
                }
            }
        }

        return mVehicleInspectionAnswer;
    }

    private boolean printTerminationSlip(String slipMessage) {

        try {
            if (TextUtils.isEmpty(SessionModel.getInstance().getPrinterMacAddress()) == false)
            {
                InspectionTerminateSlip InspectionTerminateSlip = new InspectionTerminateSlip(SessionModel.getInstance().getPrinterMacAddress(), this.getContext(), this);
                InspectionTerminateSlip.print(
                        false,
                        wizardActivity().getBookingReferenceNumber(),
                        wizardActivity().getVehicleInspectionQuery().getSiteName(),
                        wizardActivity().getVehicleInspectionQuery().getTestCategory() == null ? Constants.EMPTY_STRING : wizardActivity().getVehicleInspectionQuery().getTestCategory(),
                        slipMessage);
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionReviewFragment::printVehicleInspectionSlip()"), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
            return false;
        }

        return true;
    }

    private boolean saveVehicleInspectionResult(){

        try {

            read();
            List<VehicleInspectionResultModel> vehicleInspectionResultList = wizardActivity().getVehicleInspectionResultList();
            VehicleInspectionResultsRepository.createOrUpdate(
                    getVehicleInspectionResults(
                            vehicleInspectionResultList.get(0).getBookingID(),
                            wizardActivity().getWizardStartTime(),
                            wizardActivity().getWizardEndTime(),
                            vehicleInspectionResultList));

            EvidenceRepository.setEvidenceToSubmit(vehicleInspectionResultList.get(0).getBookingID());

            wizardActivity().enableNextButton(false);
            return true;

        }catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "saveVehicleInspectionResult::saveVehicleInspectionResult() 1"), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
            return false;
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionReviewFragment::saveVehicleInspectionResult() 2"), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
            return false;
        }
    }

    private VehicleInspectionResultsModel getVehicleInspectionResults(long bookingID, Date testStartTime, Date testEndTime, List<VehicleInspectionResultModel> vehicleInspectionResultList){

        VehicleInspectionResultsModel vehicleInspectionResults = new VehicleInspectionResultsModel();

        vehicleInspectionResults.setCredentialID(SessionModel.getInstance().getUser().getCredentialID());
        vehicleInspectionResults.setTestStartTime(testStartTime);
        vehicleInspectionResults.setTestEndTime(testEndTime);
        vehicleInspectionResults.setBookingID(bookingID);
        vehicleInspectionResults.setIsPassed(calculateIsPassedFromQuestionWeight(vehicleInspectionResultList));
        vehicleInspectionResults.setVehicleInspectionResultList(vehicleInspectionResultList);

        return vehicleInspectionResults;
    }

    private boolean calculateIsPassedFromQuestionWeight(List<VehicleInspectionResultModel>  vehicleInspectionResultList){

        double result = 0;

        for(VehicleInspectionResultModel vehicleInspectionResult : vehicleInspectionResultList){
            if (vehicleInspectionResult.getIsPassed() == 0) {
                result = result + vehicleInspectionResult.getWeight();
            }
        }

        return result < Constants.TERMINATE_INSPECTION_WEIGHT;
    }

    private void processUpdates() {

        try {
            DataSynchronisation dataSynchronisation = new DataSynchronisation(this, true);
            dataSynchronisation.processUpdates(true);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionReviewFragment::processUpdates()"), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
        }
    }

    @Override
    public void message(String message, boolean append) {
        if (message.equals(Constants.FINISHED_MESSAGE) == false) {
            MessageManager.showMessage(message, ErrorSeverity.None);
        }
    }

    private WizardActivity wizardActivity(){
        return (WizardActivity) getActivity();
    }
}
