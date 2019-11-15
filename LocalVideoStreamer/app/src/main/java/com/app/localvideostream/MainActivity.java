package com.app.localvideostream;

import android.graphics.Bitmap;
import android.os.Bundle;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.app.localvideostream.mqtt.MQTTPresenter;
import com.app.localvideostream.util.Constants;
import com.app.localvideostream.util.Utils;

import org.eclipse.paho.android.service.MqttAndroidClient;
import org.eclipse.paho.client.mqttv3.IMqttDeliveryToken;
import org.eclipse.paho.client.mqttv3.MqttCallback;
import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttMessage;

public class MainActivity extends AppCompatActivity {

    ImageView imgLoader;

    MqttAndroidClient client;
    MQTTPresenter presenter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        imgLoader = findViewById(R.id.image);
        initClient();
        presenter = new MQTTPresenter(this);
        presenter.connectToMqtt(client);
    }

    void initClient () {
        final String clientId = MqttClient.generateClientId();
        client = new MqttAndroidClient(getApplication(), Constants.broker, clientId);
        client.setCallback(new MqttCallback() {
            @Override
            public void connectionLost(Throwable cause) {
                presenter.connectToMqtt(client);
            }

            @Override
            public void messageArrived(String topic, MqttMessage message) throws Exception {

            }

            @Override
            public void deliveryComplete(IMqttDeliveryToken token) {

            }
        });
    }

    public void onSuccess(final String msg) {
        Thread thread = new Thread() {
            public void run() {
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        Toast.makeText(MainActivity.this, msg, Toast.LENGTH_LONG).show();
                    }
                });
            }
        };
        thread.start();
    }

    public void receiveImage(final byte[] payload) {
        Thread thread = new Thread() {
            public void run() {
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        Bitmap bmp = Utils.convertByteToBitmap(MainActivity.this, payload);
                        imgLoader.setImageBitmap(Bitmap.createScaledBitmap(bmp, imgLoader.getWidth(),
                                imgLoader.getHeight(), false));
                    }
                });
            }
        };
        thread.start();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        presenter.unSubscribeMqttChannel(client);
        presenter.disconnectMqtt(client);
    }
}
