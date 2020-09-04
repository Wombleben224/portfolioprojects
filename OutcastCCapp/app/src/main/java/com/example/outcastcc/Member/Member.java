package com.example.outcastcc.Member;

import androidx.annotation.NonNull;

public class Member {

    private String name;
    private String username;
    private String vehicleYear;
    private String vehicleMake;
    private String vehicleModel;
    private String bio;
    private String image;
    private String link;

    public Member(@NonNull String username, @NonNull String Name, @NonNull String vehicleYear, String vehicleMake, String vehicleModel, String bio, String image, String link){
        this.name = Name;
        this.username = username;
        this.vehicleYear = vehicleYear;
        this.vehicleMake = vehicleMake;
        this.vehicleModel = vehicleModel;
        this.bio = bio;
        this.image = image;
        this.link = link;
    }


    public String getName() {
        return name;
    }

    public String getVehicleYear() {
        return vehicleYear;
    }

    public String getVehicleMake() {
        return vehicleMake;
    }

    public String getVehicleModel() {
        return vehicleModel;
    }

    public String getBio() {
        return bio;
    }

    public String getImage() {
        return image;
    }

    public String getLink() {
        return link;
    }

    public String getUsername() {
        return username;
    }
}
