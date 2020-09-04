package com.example.outcastcc.Event;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.recyclerview.widget.RecyclerView;

import com.example.outcastcc.R;

import java.util.List;

public class EventsListAdapter extends RecyclerView.Adapter<EventsListAdapter.EventViewHolder> {
    private  Context context;
    public  class EventViewHolder extends RecyclerView.ViewHolder implements View.OnClickListener{
        public final TextView mTitleView;
        public final TextView mDescriptionView;
        public final TextView mDateView;
        public final ConstraintLayout mConstraintLayout;
        public final TextView mLocationView;
        public EventViewHolder(View itemView){
            super(itemView);
            mTitleView = itemView.findViewById(R.id.member_name);
            mDescriptionView = itemView.findViewById(R.id.member_bio);
            mDateView = itemView.findViewById(R.id.member_username);
            mConstraintLayout = itemView.findViewById(R.id.itemConstraint);
            mLocationView = itemView.findViewById(R.id.member_vehicle);
            mConstraintLayout.setOnClickListener(this);
        }

        @Override
        public void onClick(View v) {
            int index = getLayoutPosition();

            Event event = mItems.get(index);

            Intent intent = new Intent(Intent.ACTION_VIEW);
            intent.setData(Uri.parse(event.getLink()));
            context.startActivity(intent);

            EventsListAdapter.this.notifyItemChanged(index);
    }
    }


    private List<Event> mItems;
    private final LayoutInflater mInflater;

    public EventsListAdapter(Context context, List<Event> items){
        mItems = items;
        mInflater = LayoutInflater.from(context);
        this.context = context;
    }

    public void setItems(List<Event> items){
        mItems = items;
        notifyDataSetChanged();
    }


    @Override
    public int getItemCount(){
        return mItems != null ? mItems.size() : 0;
    }

    @NonNull
    @Override
    public EventViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType){
        View itemView = mInflater.inflate(R.layout.item_event, parent, false);
        return new EventViewHolder(itemView);
    }

    @Override
    public void onBindViewHolder(@NonNull EventViewHolder holder, int position){
        if (mItems != null){
            Event item = mItems.get(position);
            holder.mTitleView.setText(item.getTitle());
            holder.mDescriptionView.setText(item.getDescription());
            holder.mDateView.setText(item.getDate());
            holder.mLocationView.setText(item.getLocation());
        } else {
            holder.mTitleView.setText("");
        }

    }


}
