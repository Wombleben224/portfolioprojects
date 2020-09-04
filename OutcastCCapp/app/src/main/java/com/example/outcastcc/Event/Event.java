package com.example.outcastcc.Event;

import androidx.annotation.NonNull;

import java.text.SimpleDateFormat;
import java.util.Calendar;

public class Event {

    private String title;
    private String description;
    private String date;
    private String location;
    private String link;

    public Event(@NonNull String title, @NonNull String description, @NonNull String date, String location, String link){
        this.description = description;
        this.title = title;
        this.date = date;
        this.location = location;
        this.link = link;
    }

    public String getTitle(){ return title; }

    public String getDescription() { return description; }

    public String getLocation() {
        return location;
    }

    public String getLink() {
        return link;
    }

    public String getDate() {
        SimpleDateFormat formatter = new SimpleDateFormat("dd/MM/yyyy");

        // Create a calendar object that will convert the date and time value in milliseconds to date.
        Calendar calendar = Calendar.getInstance();
        calendar.setTimeInMillis(Long.parseLong(date.substring(date.indexOf("(")+1,date.indexOf(")"))));
        return formatter.format(calendar.getTime());
    }
}
