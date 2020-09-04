package com.example.outcastcc.Event;

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
public class EventsFragment extends Fragment {

    private RecyclerView mRecyclerView;
    private RequestQueue mrequestQueue;
    private EventsListAdapter mAdapter;
    private ArrayList<Event> mItems;

    public EventsFragment() {
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

        loadEvents();
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.event_fragment, container, false);
    }


    public void loadEvents() {
        {
            StringRequest stringRequest = new StringRequest(Request.Method.GET, OutcastApp.EVENTS_API_URL, new Response.Listener<String>() {
                @Override
                public void onResponse(String response) {
                    try {
                        JSONArray jsonObject = new JSONArray(response);
                        for (int i = 0; i < jsonObject.length(); i++) {
                            JSONObject jo = jsonObject.getJSONObject(i);

                            String title = jo.getString("Title");
                            String description = jo.getString("Text");
                            String url = jo.getString("EventId");
                            String date = jo.getString("EventDate");
                            String location = jo.getString("Location");


                            url = "http://iwt.ranken.edu/bwomble/final_project/Outcast/ViewEvent?Eventid=" + url;
                            mItems.add(
                                new Event(title, description, date, location, url)
                            );
                        }
                        mAdapter = new EventsListAdapter(getContext(), mItems);
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
