package com.example.outcastcc.Member;


import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.example.outcastcc.OutcastApp;
import com.example.outcastcc.R;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;


/**
 * A simple {@link Fragment} subclass.
 */
public class MemberFragment extends Fragment {

    private RecyclerView mRecyclerView;
    private RequestQueue mrequestQueue;
    private MemberListAdapter mAdapter;
    private ArrayList<Member> mItems;

    public MemberFragment() {
        // Required empty public constructor
    }


    @Override
    public void onStart() {
        super.onStart();

        mRecyclerView = getView().findViewById(R.id.recyclerView);
        mRecyclerView.setLayoutManager(new LinearLayoutManager(getContext()));
        mRecyclerView.setAdapter(mAdapter);

        mrequestQueue = Volley.newRequestQueue(getContext());
        mItems = new ArrayList<>();

        loadMembers();
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.member_fragment, container, false);
    }
    public void loadMembers() {
        {
            StringRequest stringRequest = new StringRequest(Request.Method.GET, OutcastApp.MEMBERS_API_URL, new Response.Listener<String>() {
                @Override
                public void onResponse(String response) {
                    try {
                        JSONArray jsonObject = new JSONArray(response);
                        for (int i = 0; i < jsonObject.length(); i++) {
                            JSONObject jo = jsonObject.getJSONObject(i);

                            String username = jo.getString("Username");
                            String name = jo.getString("Name");
                            String vehicleYear = jo.getString("VehicleYear");
                            String vehicleMake = jo.getString("VehicleMake");
                            String vehicleModel = jo.getString("VehicleModel");
                            String bio = jo.getString("Bio");
                            String image = jo.getString("ProfileImageName");
                            image = "http://iwt.ranken.edu/bwomble/final_project/Content/Photos/0/" + image;
                            String url = jo.getString("memberId");
                            url = "http://iwt.ranken.edu/bwomble/final_project/Outcast/MemberDetails/" + url;
                            mItems.add(
                                new Member(username, name, vehicleYear,vehicleMake, vehicleModel, bio, image, url)
                            );
                        }
                        mAdapter = new MemberListAdapter(getContext(), mItems);
                        mRecyclerView.setAdapter(mAdapter);
                        mRecyclerView.setLayoutManager(new LinearLayoutManager(getContext()));
                    } catch (JSONException e) {
                        Toast.makeText(getContext(), R.string.toast_error, Toast.LENGTH_SHORT).show();
                    }
                }
            }, new Response.ErrorListener() {
                @Override
                public void onErrorResponse(VolleyError error) {
                    Toast.makeText(getContext(), R.string.toast_error, Toast.LENGTH_SHORT).show();
                }
            });
            mrequestQueue.add(stringRequest);
        }
    }
}
