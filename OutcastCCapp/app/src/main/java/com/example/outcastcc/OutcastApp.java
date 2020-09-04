package com.example.outcastcc;

import android.app.Application;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.job.JobInfo;
import android.app.job.JobScheduler;
import android.content.ComponentName;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.util.Log;

import androidx.core.app.NotificationCompat;

import com.example.outcastcc.Event.EventsService;
import com.example.outcastcc.Member.MemberService;

public class OutcastApp extends Application {
    public static final String EVENTS_API_URL = "http://iwt.ranken.edu/bwomble/final_project/Outcast/GetAllEvents";
    public static final String MEMBERS_API_URL = "http://iwt.ranken.edu/bwomble/final_project/Outcast/GetAllMembers";


    private static final String LOG_TAG = OutcastApp.class.getSimpleName();
    public static final String PRIMARY_CHANNEL_ID = "primary_channel_id";
    public static final int NOTIFICATION_ID = 0;
    public static final int EVENT_JOB_ID = 0;
    public static final int MEMBER_JOB_ID = 1;
    public static final String NEW_EVENTS_ACTION = "com.example.outcastcc.NEW_EVENTS_ACTION";
    public static final String NEW_MEMBER_ACTION = "com.example.outcastcc.NEW_MEMBER_ACTION";
    public static final String EXTRA_MESSAGE = "message";
    public static final String EXTRA_LAST_UPDATED = "lastUpdated";
    public static final String EXTRA_MEMBER_USERNAME = "Username";


    private NotificationManager mNotifyManager;
    private JobScheduler mScheduler;
    private String mEventLastUpdated;
    private String mLastMember;

    @Override
    public void onCreate(){
        super.onCreate();

        mNotifyManager = (NotificationManager)getSystemService(NOTIFICATION_SERVICE);
        mScheduler = (JobScheduler)getSystemService(JOB_SCHEDULER_SERVICE);

        createNotificationChannel();
        scheduleEventJob();
        scheduleMemberJob();
        Log.i(LOG_TAG, "APP STARTED");
    }

    public void createNotificationChannel(){
        if (Build.VERSION.SDK_INT >= 26){
            NotificationChannel channel =
                new NotificationChannel(
                    PRIMARY_CHANNEL_ID,
                    getString(R.string.primary_channel_name),
                    NotificationManager.IMPORTANCE_HIGH);

            channel.setDescription(getString(R.string.primary_channel_description));
            mNotifyManager.createNotificationChannel(channel);
        }
    }

    public void sendMemberNotification(String Name, String url){
        if (mLastMember == null || mLastMember.equals(Name)){
            mLastMember = Name;
            return;
        }
        NotificationCompat.Builder builder =
            new NotificationCompat.Builder(this, PRIMARY_CHANNEL_ID);

        builder.setStyle(new NotificationCompat.BigTextStyle());
        builder.setSmallIcon(R.drawable.ic_android);
        builder.setContentTitle("New Member");
        builder.setContentText(Name);
        builder.setPriority(NotificationCompat.PRIORITY_HIGH);
        builder.setDefaults(NotificationCompat.DEFAULT_ALL);

        Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setData(Uri.parse(url));

        PendingIntent pendingContentIntent =
            PendingIntent.getActivity(this, NOTIFICATION_ID, intent, PendingIntent.FLAG_UPDATE_CURRENT);


        builder.setContentIntent(pendingContentIntent);
        builder.setAutoCancel(true);

        mNotifyManager.notify(NOTIFICATION_ID, builder.build());
        mLastMember = Name;
    }

    public void sendEventNotification(String title, String lastUpdated, String url){
        if (mEventLastUpdated == null || mEventLastUpdated.equals(lastUpdated)){
            mEventLastUpdated = lastUpdated;
            return;
        }
        NotificationCompat.Builder builder =
        new NotificationCompat.Builder(this, PRIMARY_CHANNEL_ID);

        builder.setStyle(new NotificationCompat.BigTextStyle());
        builder.setSmallIcon(R.drawable.ic_android);
        builder.setContentTitle("New Event");
        builder.setContentText(title);
        builder.setPriority(NotificationCompat.PRIORITY_HIGH);
        builder.setDefaults(NotificationCompat.DEFAULT_ALL);

        Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setData(Uri.parse(url));

        PendingIntent pendingContentIntent =
            PendingIntent.getActivity(this, NOTIFICATION_ID, intent, PendingIntent.FLAG_UPDATE_CURRENT);


        builder.setContentIntent(pendingContentIntent);
        builder.setAutoCancel(true);

        mNotifyManager.notify(NOTIFICATION_ID, builder.build());
        mEventLastUpdated = lastUpdated;
    }

    public void scheduleMemberJob(){
        ComponentName serviceName =
            new ComponentName(getPackageName(), MemberService.class.getName());

        JobInfo.Builder builder = new JobInfo.Builder(MEMBER_JOB_ID, serviceName);
        builder.setRequiredNetworkType(JobInfo.NETWORK_TYPE_ANY);

        builder.setPeriodic(15 * 60 * 1000);
        builder.setPersisted(true);

        mScheduler.schedule(builder.build());
    }

    public void scheduleEventJob(){
        ComponentName serviceName =
            new ComponentName(getPackageName(), EventsService.class.getName());

        JobInfo.Builder builder = new JobInfo.Builder(EVENT_JOB_ID, serviceName);
        builder.setRequiredNetworkType(JobInfo.NETWORK_TYPE_ANY);

        builder.setPeriodic(15 * 60 * 1000);
        builder.setPersisted(true);

        mScheduler.schedule(builder.build());
    }
}
