package com.app.localvideostream;

import android.graphics.Bitmap;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.widget.ImageView;

import androidx.appcompat.app.AppCompatActivity;

import com.app.localvideostream.util.Utils;

import java.io.ByteArrayOutputStream;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.SocketException;

public class MainActivity extends AppCompatActivity {

    ImageView imgLoader;
    DatagramSocket udpSocket;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        imgLoader = findViewById(R.id.image);

        try {
            udpSocket = new DatagramSocket(12346);
        } catch (SocketException e) {
            e.printStackTrace();
        }
    }

    @Override
    protected void onResume() {
        runListner();
        super.onResume();
    }

    public void receiveImage(byte[] payload) {
        Bitmap bmp = Utils.convertByteToBitmap(MainActivity.this, payload);
        if (bmp != null) {
            imgLoader.setImageBitmap(Bitmap.createScaledBitmap(bmp, imgLoader.getWidth(),
                    imgLoader.getHeight(), false));
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
    }

    public void runListner() {
        new ReadSocket().execute();
    }

    private class ReadSocket extends AsyncTask<Void, Void, String> {
        ByteArrayOutputStream baos;
        @Override
        protected String doInBackground(Void... voids) {
            try {
                udpSocket.setReuseAddress(true);
                udpSocket.setBroadcast(true);
                byte[] message = new byte[1024 * 64];
                DatagramPacket packet = new DatagramPacket(message,message.length);
                Log.i("UDP client: ", "about to wait to receive");
                udpSocket.receive(packet);
                String cnt_str = new String(message, 0, packet.getLength());
                int cnt = Integer.parseInt(cnt_str);
                Log.d("udp length: ", cnt_str);
                baos = new ByteArrayOutputStream();
                for (int i = 0; i < cnt; i ++){
                    udpSocket.receive(packet);
                    baos.write(Utils.subArray(message, 0, packet.getLength()));
                }
                Log.d("str", "sdf");
            }catch (Exception e) {
                Log.e("IO Exception", "error: ", e);
            }
            return null;
        }

        @Override
        protected void onPostExecute(String result) {
            receiveImage(baos.toByteArray());
            runListner();
        }
    }
}