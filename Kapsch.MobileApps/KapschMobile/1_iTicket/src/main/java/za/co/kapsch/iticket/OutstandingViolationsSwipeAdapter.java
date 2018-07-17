package za.co.kapsch.iticket;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;

import za.co.kapsch.iticket.Enums.ViolationCategory;
import za.co.kapsch.iticket.Models.SwipeAdapterTabTitle;

/**
 * Created by CSenekal on 2018/04/17.
 */

public class OutstandingViolationsSwipeAdapter extends FragmentPagerAdapter {

    private SwipeAdapterTabTitle[] mTabTitles;// = new String[]{
            //App.getContext().getResources().getString(R.string.person_ex),
            //App.getContext().getResources().getString(R.string.vehicle)
    //};

    public OutstandingViolationsSwipeAdapter(FragmentManager fm) {
        super(fm);
    }

    public OutstandingViolationsSwipeAdapter(FragmentManager fm, SwipeAdapterTabTitle[] tabTitles) {
        super(fm);
        mTabTitles = tabTitles;
    }

//    @Override
//    public Fragment getItem(int position) {
//
//        Fragment fragment = null;
//
//        switch (position) {
//            case 0:
//                fragment = new OutstandingViolationsFragment();
//                setBundle(fragment, ViolationCategory.PERSON);
//                break;
//            case 1:
//                fragment = new OutstandingViolationsFragment();
//                setBundle(fragment, ViolationCategory.VEHICLE);
//                break;
//        }
//
//        return fragment;
//    }

    @Override
    public Fragment getItem(int position) {

        Fragment fragment = null;

        fragment = new OutstandingViolationsFragment();
        setBundle(fragment, mTabTitles[position].getTabIdentifier());


//        switch (position) {
//            case 0:
//                fragment = new OutstandingViolationsFragment();
//                setBundle(fragment, ViolationCategory.PERSON);
//                break;
//            case 1:
//                fragment = new OutstandingViolationsFragment();
//                setBundle(fragment, ViolationCategory.VEHICLE);
//                break;
//        }

        return fragment;
    }

    @Override
    public int getCount() {
        return mTabTitles.length;
    }

    @Override
    public CharSequence getPageTitle(int position) {

        return mTabTitles[position].getTabTitle();
    }

    private void setBundle(Fragment fragment, int tabIdentifier){

        Bundle bundle = new Bundle();
        bundle.putInt(Constants.VEHICLE_CATEGORY, tabIdentifier);
        fragment.setArguments(bundle);
    }

//    private void setBundle(Fragment fragment, ViolationCategory violationCategory){
//        Bundle bundle = new Bundle();
//        bundle.putSerializable(Constants.VEHICLE_CATEGORY, violationCategory);
//        fragment.setArguments(bundle);
//    }
}

