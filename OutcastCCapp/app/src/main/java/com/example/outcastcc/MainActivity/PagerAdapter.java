package com.example.outcastcc.MainActivity;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentPagerAdapter;

import com.example.outcastcc.Member.MemberFragment;
import com.example.outcastcc.Event.EventsFragment;

public class PagerAdapter extends FragmentPagerAdapter {

    public PagerAdapter(@NonNull FragmentManager fm) {
        super(fm);
    }

    @NonNull
    @Override
    public Fragment getItem(int position) {
        switch (position){
            case 0: return new HomeFragment();
            case 1: return new EventsFragment();
            case 2: return new MemberFragment();
            default: throw new IndexOutOfBoundsException("position is invalid");
        }
    }

    @Override
    public int getCount() {
        return 3;
    }
}
