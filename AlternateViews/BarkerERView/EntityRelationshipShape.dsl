﻿<?xml version="1.0" encoding="utf-8"?>
<!--
	Neumont Object-Role Modeling Architect for Visual Studio

	Copyright © Neumont University. All rights reserved.

	The use and distribution terms for this software are covered by the
	Common Public License 1.0 (http://opensource.org/licenses/cpl) which
	can be found in the file CPL.txt at the root of this distribution.
	By using this software in any fashion, you are agreeing to be bound by
	the terms of this license.

	You must not remove this notice, or any other, from this software.
-->
<Dsl
	xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	PackageGuid="EFDDC549-1646-4451-8A51-E5A5E94D647C"
	Id="2E6C7361-69AE-411A-BB82-09F844E6BA95"
	Namespace="Neumont.Tools.ORM.Views.BarkerERView"
	Name="BarkerERShape"
	DisplayName="Barker ER View"
	Description="(Preliminary) Graphical View of Barker ER Model"
	CompanyName="Neumont University"
	ProductName="Neumont ORM Architect for Visual Studio"
	MajorVersion="1" MinorVersion="0" Build="0" Revision="0"
	AccessModifier="Assembly">
	<Attributes>
		<ClrAttribute Name="DslModeling::ExtendsDomainModel">
			<Parameters>
				<AttributeParameter Value="&quot;77C1024F-D688-4AEE-AF16-29C2E791A9E7&quot;/*Neumont.Tools.ORMAbstractionToBarkerERBridge.ORMAbstractionToBarkerERBridgeDomainModel*/"/>
			</Parameters>
		</ClrAttribute>
		<ClrAttribute Name="DslModeling::ExtendsDomainModel">
			<Parameters>
				<AttributeParameter Value="&quot;C52FB9A5-6BF4-4267-8716-71D74C7AA89C&quot;/*Neumont.Tools.ORM.ShapeModel*/"/>
			</Parameters>
		</ClrAttribute>
	</Attributes>

	<Shapes>
		<CompartmentShape Name="BarkerEntityShape" Id="3AD5BD5C-F5A1-46C1-9F82-2110DBFC69EB" Namespace="Neumont.Tools.ORM.Views.BarkerERView" 
										GeneratesDoubleDerived="true" InitialHeight="0.3" InitialWidth="1" OutlineThickness="0.015625" 
										AccessModifier="Assembly" Geometry="RoundedRectangle" IsSingleCompartmentHeaderVisible="false">
			<Properties>
				<DomainProperty Name="UpdateCounter" Id="E0BCB4FF-7C01-431A-8178-583E733D13AE" GetterAccessModifier="Private" SetterAccessModifier="Private" Kind="CustomStorage" IsBrowsable="false">
					<Type>
						<ExternalTypeMoniker Name="/System/Int64"/>
					</Type>
				</DomainProperty>
			</Properties>
			<ShapeHasDecorators Position="InnerTopCenter">
				<TextDecorator FontSize="10" FontStyle="Bold" Name="BarkerEntityNameDecorator"/>
			</ShapeHasDecorators>
			<Compartment Name="AttributesCompartment" Title="Attributes"/>
		</CompartmentShape>
	</Shapes>

	<Connectors>
		<Connector Id="03FE4DA6-29DC-4DFC-9450-07E7D2B5EC48" Name="AssociationConnector" Namespace="Neumont.Tools.ORM.Views.BarkerERView" DisplayName="AssociationConnector" 
			Thickness="0.01">
			<BaseConnector>
				<ConnectorMoniker Name="/Neumont.Tools.ORM.ShapeModel/ORMBaseBinaryLinkShape"/>
			</BaseConnector>
		</Connector>
	</Connectors>
	
	<XmlSerializationBehavior Name="BarkerEntityShapeModelSerializationBehavior" Namespace="Neumont.Tools.ORM.Views.BarkerERView"/>

	<!-- Diagram is double-derived so that we can override CreateChildShape. -->
	<Diagram Name="BarkerERDiagram" Namespace="Neumont.Tools.ORM.Views.BarkerERView" Id="144546C5-DB5E-4A4C-B1BC-4B7C6C0612F2" AccessModifier="Assembly" GeneratesDoubleDerived="true" HasCustomConstructor="true">
		<Class>
			<DomainClassMoniker Name="/Neumont.Tools.EntityRelationshipModels.Barker/BarkerErModel"/>
		</Class>
		<ShapeMaps>
			<CompartmentShapeMap>
				<DomainClassMoniker Name="/Neumont.Tools.EntityRelationshipModels.Barker/EntityType"/>
				<ParentElementPath>
					<DomainPath>Neumont.Tools.EntityRelationshipModels.Barker.BarkerErModelContainsEntityType.BarkerErModel/!BarkerErModel</DomainPath>
				</ParentElementPath>
				<DecoratorMap>
					<TextDecoratorMoniker Name="BarkerEntityShape/BarkerEntityNameDecorator"/>
					<PropertyDisplayed>
						<PropertyPath>
							<DomainPropertyMoniker Name="/Neumont.Tools.EntityRelationshipModels.Barker/EntityType/Name"/>
						</PropertyPath>
					</PropertyDisplayed>
				</DecoratorMap>
				<CompartmentShapeMoniker Name="BarkerEntityShape"/>
				<CompartmentMap>
					<CompartmentMoniker Name="BarkerEntityShape/AttributesCompartment" />
					<ElementsDisplayed>
						<DomainPath>Neumont.Tools.EntityRelationshipModels.Barker.EntityTypeHasAttribute.AttributeCollection/!Attribute</DomainPath>
					</ElementsDisplayed>
					<PropertyDisplayed>
						<PropertyPath>
							<DomainPropertyMoniker Name="/Neumont.Tools.EntityRelationshipModels.Barker/Attribute/Name" />
						</PropertyPath>
					</PropertyDisplayed>
				</CompartmentMap>
			</CompartmentShapeMap>
		</ShapeMaps>
		<ConnectorMaps>
			<!-- This one needs custom code, there is an extra step (TableContainsReferenceConstraint) required to
			find the source shape. -->
			<!-- <ConnectorMap>
				<ConnectorMoniker Name="AssociationConnector"/>
				<DomainRelationshipMoniker Name="/Neumont.Tools.EntityRelationshipModels.Barker/BarkerErModelContainsBinaryAssociation"/>
			</ConnectorMap> -->
		</ConnectorMaps>
	</Diagram>

	<Designer EditorGuid="62FFBFDA-C1C8-409A-9DA6-85EFC20DBA12">
		<RootClass>
			<DomainClassMoniker Name="/Neumont.Tools.EntityRelationshipModels.Barker/BarkerErModel"/>
		</RootClass>
		<XmlSerializationDefinition>
			<XmlSerializationBehaviorMoniker Name="/Neumont.Tools.ORM.Views.BarkerERView/BarkerEntityShapeModelSerializationBehavior"/>
		</XmlSerializationDefinition>
		<DiagramMoniker Name="BarkerERDiagram"/>
	</Designer>

</Dsl>