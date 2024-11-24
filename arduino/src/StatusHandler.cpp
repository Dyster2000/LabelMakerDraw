/*
This file is part of LabelMakerDrawBluetooth.

LabelMakerDrawBluetooth is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation version 3 or later.

LabelMakerDrawBluetooth is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with LabelMakerDrawBluetooth. If not, see <https://www.gnu.org/licenses/>.
*/
#include "StatusHandler.h"
#include "LabelmakerBleServer.h"
#include <Arduino.h>
#include <BLEUtils.h>
#include <BLE2902.h>

StatusHandler::StatusHandler(LabelMakerStatus &data, LabelmakerBleServer &server, BLEService &service)
  : m_Data(data)
  , m_Server(server)
{
  // Create a BLE Characteristic
  Serial.println("[StatusHandler] Create characteristic");
  m_pCharacteristic = service.createCharacteristic(
      CHARACTERISTIC_UUID,
      BLECharacteristic::PROPERTY_READ | BLECharacteristic::PROPERTY_NOTIFY | BLECharacteristic::PROPERTY_INDICATE);

  // Create a BLE Descriptor
  m_pCharacteristic->addDescriptor(new BLE2902());
}

void StatusHandler::Loop()
{
  if (m_Server.IsConnected())
  {
    if (m_LastData.State != m_Data.State)
    {
      m_LastData = m_Data;

      Serial.print("[StatusHandler::Loop] New state: ");
      Serial.println(m_Data.State);

      m_pCharacteristic->setValue((uint8_t *)&m_Data, sizeof(LabelMakerStatus));
      m_pCharacteristic->notify();
    }
  }
}