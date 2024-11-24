/*
This file is part of LabelMakerDrawBluetooth.

LabelMakerDrawBluetooth is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation version 3 or later.

LabelMakerDrawBluetooth is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with LabelMakerDrawBluetooth. If not, see <https://www.gnu.org/licenses/>.
*/
#pragma once

#include <BLEDevice.h>
#include <BLEServer.h>
#include "LabelMakerData.h"

class LabelmakerBleServer;

class StatusHandler
{
public:
  StatusHandler(LabelMakerStatus &data, LabelmakerBleServer &server, BLEService &service);

  void Loop();

private:
  const char *CHARACTERISTIC_UUID = "29fac492-6d43-4fa3-8929-3f902a056dfb";

  LabelMakerStatus &m_Data;
  LabelmakerBleServer &m_Server;
  BLECharacteristic *m_pCharacteristic;

  LabelMakerStatus m_LastData;
};