<?xml version="1.0" encoding="utf-8"?>
<savedscenario>
	<meta>
		<gameVersion>1.6.4633 rev1261</gameVersion>
		<modIds>
			<li>brrainz.harmony</li>
			<li>ludeon.rimworld</li>
			<li>acct.pvzrimworld</li>
			<li>brrainz.cameraplus</li>
			<li>void.charactereditor</li>
			<li>latta.devl10n</li>
			<li>latta.devl10n.zh</li>
		</modIds>
		<modSteamIds>
			<li>0</li>
			<li>0</li>
			<li>0</li>
			<li>867467808</li>
			<li>0</li>
			<li>0</li>
			<li>0</li>
		</modSteamIds>
		<modNames>
			<li>Harmony</li>
			<li>Core</li>
			<li>PVZRimWorld</li>
			<li>Camera+</li>
			<li>Character Editor</li>
			<li>Dev In Your Language</li>
			<li>Dev In Your Language_zh</li>
		</modNames>
	</meta>
	<scenario>
		<name>保护好你的脑子！！</name>
		<summary>在一场又一场大型的僵尸袭击中存活下来！</summary>
		<description>四个人在在一场大型的僵尸袭击中存活下来，我们安全了，暂时...
也许你手中的种子能够给你带来好运，而僵尸很快会赶上你！做点什么！！</description>
		<playerFaction>
			<def>PlayerFaction</def>
			<factionDef>PlayerColony</factionDef>
		</playerFaction>
		<surfaceLayer>
			<def>SurfaceLayerFixed</def>
			<layer>Surface</layer>
			<settingsDef>Surface</settingsDef>
			<hide>True</hide>
			<tag>Surface</tag>
			<connections />
		</surfaceLayer>
		<parts>
			<li Class="ScenPart_ConfigPage_ConfigureStartingPawns">
				<def>ConfigPage_ConfigureStartingPawns</def>
				<pawnChoiceCount>8</pawnChoiceCount>
				<pawnCount>4</pawnCount>
			</li>
			<li Class="ScenPart_PlayerPawnsArriveMethod">
				<def>PlayerPawnsArriveMethod</def>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>Silver</thingDef>
				<count>800</count>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>MealSurvivalPack</thingDef>
				<count>50</count>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>MedicineIndustrial</thingDef>
				<count>30</count>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>ComponentIndustrial</thingDef>
				<count>30</count>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>Gun_MachinePistol</thingDef>
				<count>2</count>
				<quality>Normal</quality>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>Gun_AssaultRifle</thingDef>
				<quality>Good</quality>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>Gun_LMG</thingDef>
				<quality>Excellent</quality>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>Turret_MiniTurret</thingDef>
				<stuff>Steel</stuff>
				<count>4</count>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>Apparel_FlakVest</thingDef>
				<count>4</count>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>Apparel_AdvancedHelmet</thingDef>
				<stuff>Plasteel</stuff>
				<count>4</count>
			</li>
			<li Class="ScenPart_StartingAnimal">
				<def>StartingAnimal</def>
				<count>1</count>
				<bondToRandomPlayerPawnChance>1</bondToRandomPlayerPawnChance>
			</li>
			<li Class="ScenPart_ScatterThingsNearPlayerStart">
				<def>ScatterThingsNearPlayerStart</def>
				<thingDef>Steel</thingDef>
				<count>600</count>
				<allowRoofed>True</allowRoofed>
			</li>
			<li Class="ScenPart_ScatterThingsNearPlayerStart">
				<def>ScatterThingsNearPlayerStart</def>
				<thingDef>WoodLog</thingDef>
				<count>400</count>
				<allowRoofed>True</allowRoofed>
			</li>
			<li Class="ScenPart_ScatterThingsAnywhere">
				<def>ScatterThingsAnywhere</def>
				<thingDef>ShipChunk</thingDef>
				<count>5</count>
			</li>
			<li Class="ScenPart_ScatterThingsAnywhere">
				<def>ScatterThingsAnywhere</def>
				<thingDef>Steel</thingDef>
				<count>720</count>
				<allowRoofed>True</allowRoofed>
			</li>
			<li Class="ScenPart_ScatterThingsAnywhere">
				<def>ScatterThingsAnywhere</def>
				<thingDef>MealSurvivalPack</thingDef>
				<count>10</count>
				<allowRoofed>True</allowRoofed>
			</li>
			<li Class="ScenPart_GameStartDialog">
				<def>GameStartDialog</def>
				<text>四个人在在一场大型的僵尸袭击中存活下来，我们安全了，暂时...
也许你手中的种子能够给你带来好运，而僵尸很快会赶上你！做点什么！！</text>
				<textKey>GameStartDialog</textKey>
				<closeSound>GameStartSting</closeSound>
			</li>
			<li Class="ScenPart_CreateIncident">
				<def>CreateIncident</def>
				<incident>RaidEnemy</incident>
				<intervalDays>34.222023</intervalDays>
				<minDays>3</minDays>
				<maxDays>3</maxDays>
			</li>
			<li Class="ScenPart_StartingResearch">
				<def>StartingResearch</def>
				<project>PlantCultivation</project>
			</li>
			<li Class="ScenPart_StartingResearch">
				<def>StartingResearch</def>
				<project>Electricity</project>
			</li>
			<li Class="ScenPart_StartingThing_Defined">
				<def>StartingThing_Defined</def>
				<thingDef>Sunlight</thingDef>
				<count>500</count>
			</li>
		</parts>
	</scenario>
</savedscenario>