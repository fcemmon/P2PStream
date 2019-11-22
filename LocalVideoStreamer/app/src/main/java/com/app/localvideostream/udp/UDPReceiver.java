package com.app.localvideostream.udp;

import android.util.Log;

import java.net.DatagramPacket;
import java.net.DatagramSocket;

public class UDPReceiver implements Runnable {
    private final static int LISTENING_PORT = 12346;

    @Override
    public void run() {
        try {
            //Opening listening socket
            Log.d("UDP Receiver", "Opening listening socket on port "+LISTENING_PORT+"...");
            DatagramSocket socket = new DatagramSocket(LISTENING_PORT);
            socket.setBroadcast(true);
            socket.setReuseAddress(true);

            while(true){
                //Listening on socket
                Log.d("UDP Receiver", "Listening...");
                byte[] buf = new byte[1024 * 64];
                DatagramPacket packet = new DatagramPacket(buf, buf.length);
                socket.receive(packet);
                Log.d("UDP", "Received: '" + new String(packet.getData()).trim() + "'");
            }
        } catch (Exception e) {
            Log.e("UDP", "Receiver error", e);
        }
    }
}