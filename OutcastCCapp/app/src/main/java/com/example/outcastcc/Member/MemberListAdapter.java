package com.example.outcastcc.Member;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.recyclerview.widget.RecyclerView;

import com.example.outcastcc.R;
import com.squareup.picasso.Picasso;

import java.util.List;

public class MemberListAdapter extends RecyclerView.Adapter<MemberListAdapter.MemberViewHolder> {
    private  Context context;
    public  class MemberViewHolder extends RecyclerView.ViewHolder implements View.OnClickListener{
        public final TextView mName;
        public final TextView mBio;
        public final TextView mUserName;
        public final ConstraintLayout mConstraintLayout;
        public final TextView mVehicle;
        public final ImageView mImage;
        public MemberViewHolder(View itemView){
            super(itemView);
            mName = itemView.findViewById(R.id.member_name);
            mBio = itemView.findViewById(R.id.member_bio);
            mUserName = itemView.findViewById(R.id.member_username);
            mConstraintLayout = itemView.findViewById(R.id.itemConstraint);
            mVehicle = itemView.findViewById(R.id.member_vehicle);
            mImage = itemView.findViewById(R.id.member_image);
            mConstraintLayout.setOnClickListener(this);
        }

        @Override
        public void onClick(View v) {
            int index = getLayoutPosition();

            Member member = mItems.get(index);

            Intent intent = new Intent(Intent.ACTION_VIEW);
            intent.setData(Uri.parse(member.getLink()));
            context.startActivity(intent);

            MemberListAdapter.this.notifyItemChanged(index);
    }
    }


    private List<Member> mItems;
    private final LayoutInflater mInflater;

    public MemberListAdapter(Context context, List<Member> items){
        mItems = items;
        mInflater = LayoutInflater.from(context);
        this.context = context;
    }

    public void setItems(List<Member> items){
        mItems = items;
        notifyDataSetChanged();
    }


    @Override
    public int getItemCount(){
        return mItems != null ? mItems.size() : 0;
    }

    @NonNull
    @Override
    public MemberViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType){
        View itemView = mInflater.inflate(R.layout.item_member, parent, false);
        return new MemberViewHolder(itemView);
    }

    @Override
    public void onBindViewHolder(@NonNull MemberViewHolder holder, int position){
        if (mItems != null){
            Member item = mItems.get(position);
            holder.mName.setText(item.getName());
            holder.mBio.setText(item.getBio());
            holder.mUserName.setText(item.getUsername());
            holder.mVehicle.setText(item.getVehicleYear() + " " + item.getVehicleMake() + " " + item.getVehicleModel());
            Picasso.get()
                .load(item.getImage())
                .placeholder(R.drawable.ic_android)
                .error(R.drawable.ic_android)
                .into(holder.mImage);
        } else {
            holder.mName.setText("");
        }

    }


}
