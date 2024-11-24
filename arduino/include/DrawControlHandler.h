/*
This file is part of LabelMakerDrawBluetooth.

LabelMakerDrawBluetooth is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation version 3 or later.

LabelMakerDrawBluetooth is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with LabelMakerDrawBluetooth. If not, see <https://www.gnu.org/licenses/>.
*/
#pragma once

#include <BLEDevice.h>
#include <BLEServer.h>
#include "LabelmakerData.h"
#include <list>

class LabelmakerBleServer;

#pragma pack(1)

constexpr uint16_t HeaderSize = 4;
constexpr uint16_t MaxPoints = 5;
constexpr uint16_t PointSize = 3;

struct DrawCommandData
{
  uint16_t Offset{0};
  uint16_t TotalSize{0};
  DrawPoint Points[MaxPoints];
};

#pragma pack()

class DrawControlHandler : public BLECharacteristicCallbacks
{
public:
  DrawControlHandler(LabelMakerStatus &data, LabelmakerBleServer &server, BLEService &service);
  virtual ~DrawControlHandler() = default;

  void Loop();

  bool IsReceivingImage();
  uint8_t ReceivedPercent();
  size_t GetImagePointCount();
  DrawPoint GetPoint(size_t index);
  void ClearImage();

private:
  virtual void onWrite(BLECharacteristic *pCharacteristic, esp_ble_gatts_cb_param_t *param);

  void SetNextStep();

private:
  const char *CHARACTERISTIC_UUID = "7e623994-f779-4997-94dd-5bc1c2a1265c";
  static constexpr uint16_t MinSize = 7;
  static constexpr uint16_t MaxSize = 19;
  static constexpr float MicrosPerTurnDegree = 1.0/5000000; // 90 degrees over 5 seconds at max turn rate

  LabelMakerStatus &m_Data;
  LabelmakerBleServer &m_Server;
  BLECharacteristic *m_pCharacteristic;

  DrawCommandData m_ReceivedData;
  bool m_ReceivedDataUpdated;
  std::vector<DrawPoint> m_Points;

  int m_CurrentIndex;
  bool m_ReceivingImage;
  uint8_t m_PercentReceived;
  bool m_HaveImage;
};