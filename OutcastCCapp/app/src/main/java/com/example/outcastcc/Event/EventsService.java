package com.example.outcastcc.Event;

import android.app.job.JobParameters;
import android.app.job.JobService;
import android.content.Intent;
import android.widget.Toast;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.example.outcastcc.OutcastApp;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

public class EventsService extends JobService {

    private RequestQueue mrequestQueue;

    @Override
    public void onCreate() {
        super.onCreate();
        mrequestQueue = Volley.newRequestQueue(this);
    }

    @Override
    public boolean onStartJob(final JobParameters params) {
        StringRequest stringRequest = new StringRequest(Request.Method.GET, OutcastApp.EVENTS_API_URL, new Response.Listener<String>() {
            @Override
            public void onResponse(String response) {
                String title = null;
                try {
                    JSONArray jsonObject = new JSONArray(response);
                    JSONObject jo = jsonObject.getJSONObject(jsonObject.length() - 1);

                    title = jo.getString("Title");
                    String url = jo.getString("EventId");
                    String lastUpdated = jo.getString("EventDate");
                    url = "http://iwt.ranken.edu/bwomble/final_project/Outcast/ViewEvent?Eventid=" + url;
                    Intent broadcastIntent = new Intent(OutcastApp.NEW_EVENTS_ACTION);
                    broadcastIntent.putExtra(OutcastApp.EXTRA_MESSAGE, title);
                    broadcastIntent.putExtra(OutcastApp.EXTRA_LAST_UPDATED, lastUpdated);
                    LocalBroadcastManager
                        .getInstance(EventsService.this)
                        .sendBroadcast(broadcastIntent);

                    OutcastApp app = (OutcastApp) getApplication();
                        app.sendEventNotification(title, lastUpdated, url);
                } catch (JSONException e) {
                    showToast(e.getMessage());
                }
                finally {
                    jobFinished(params, false);
                }
            }
        }, new Response.ErrorListener() {
            @Override
            public void onErrorResponse(VolleyError error) {
                showToast(error.getMessage());
            }
        });
        mrequestQueue.add(stringRequest);

        return true;
    }

    @Override
    public boolean onStopJob(JobParameters params) {
        mrequestQueue.stop();
        showToast("Job Stopped");
        return false;
    }

    public void showToast(String message){
        Toast.makeText(this, message, Toast.LENGTH_LONG).show();
    }
}
