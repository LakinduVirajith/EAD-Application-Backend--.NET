#!/bin/bash

# REPLACE PLACEHOLDERS IN APP SETTINGS JSON
sed -i "s|\${JWT_ISSUER}|$JWT_ISSUER|g" appsettings.json
sed -i "s|\${AUDIENCE}|$AUDIENCE|g" appsettings.json
sed -i "s|\${JWT_KEY}|$JWT_KEY|g" appsettings.json
