# Notes: To each Visual Studio compilation output. We chain a few lines of scripts
# together. In doing so, Make executes all of them in one process. We want the exit 
# status of this process to be the exit status of code compilation. So we add this 
# line of code at the end. Note that there is no line continuation character "\" at
# the end of this line because this is the end of a chain of commands to be executed
# in one shell process. However, the line before this should have a "\".
#		exit $$COMPILE_STATUS
#
# On the other hand, if we put MSVC call within a if statement where the whole compilation
# binary signing and copying to distribution directory are all in one line. That means
# the command "exit $$COMPILE_STATUS" cannot be the last commnand to be executed in a
# process. In this case, we need to check exit status before calling "exit $$COMPILE_STATUS".
#		if [ $$COMPILE_STATUS -ne 0 ]; then		\
#			exit $$COMPILE_STATUS ;				\
#		fi ;									\
#

ifeq ($(NLBUILDROOT),)
  $(error NLBUILDROOT undefined.)
endif

ifeq ($(NLEXTERNALDIR),)
  $(error NLEXTERNALDIR undefined.)
endif

ifeq ($(NLEXTERNALDIR2),)
  $(error NLEXTERNALDIR2 undefined.)
endif


#
# Variables
#

PROJECT_SP2019=NextLabs.RightsManager_SP2019.csproj
PROJECT_SP2016=NextLabs.RightsManager_SP2016.csproj
PROJECT_SP2013=NextLabs.RightsManager_SP2013.csproj
TARGETS_SP=spprepare sp2019 sp2016 sp2013

ifeq ($(TARGETENVARCH),)
	TARGETENVARCH=x86
endif

ifneq ($(BUILDTYPE), release)
	BUILDTYPE=debug
endif

PROJECT_CONFIG_SP2019=sp2019$(BUILDTYPE)
PROJECT_CONFIG_SP2016=sp2016$(BUILDTYPE)
PROJECT_CONFIG_SP2013=sp2013$(BUILDTYPE)
BIN_DIR=$(BUILDTYPE)_dotnet
BUILD_LOGFILE_SP2019=$(PROJECT_CONFIG_SP2019)_build.log
BUILD_LOGFILE_SP2016=$(PROJECT_CONFIG_SP2016)_build.log
BUILD_LOGFILE_SP2013=$(PROJECT_CONFIG_SP2013)_build.log

# Makefile.vs contains target all which depends on $(TARGETS_SP)
include $(NLBUILDROOT)/build/Makefile.vs


$(info --------------------------------------------------------------------------)
$(info [Project])
$(info PROJECT_SP2019=$(PROJECT_SP2019))
$(info PROJECT_SP2016=$(PROJECT_SP2016))
$(info PROJECT_SP2013=$(PROJECT_SP2013))
$(info PROJECT_CONFIG_SP2019=$(PROJECT_CONFIG_SP2019))
$(info PROJECT_CONFIG_SP2016=$(PROJECT_CONFIG_SP2016))
$(info PROJECT_CONFIG_SP2013=$(PROJECT_CONFIG_SP2013))
$(info [Parameters])
$(info TARGETS_SP=$(TARGETS_SP))
$(info BUILDTYPE=$(BUILDTYPE))
$(info NLBUILDROOT=$(NLBUILDROOT))
$(info NLEXTERNALDIR=$(NLEXTERNALDIR))
$(info NLEXTERNALDIR2=$(NLEXTERNALDIR2))
$(info BUILDOUTPUTDIR=$(BUILDOUTPUTDIR))
$(info BIN_DIR=$(BIN_DIR))
$(info ---------------------------------------------------------------------------)

.PHONY: spprepare
spprepare:
	@echo "copy depend dll before build"
	@if [ ! -d $(NLBUILDROOT)/prod/RMXForSP/depend ]; then					\
		mkdir $(NLBUILDROOT)/prod/RMXForSP/depend ;						\
	fi
	cp -pf $(MSENFORCECOMMON)/bin/$(BUILDTYPE)_win_$(TARGETENVARCH)/QueryCloudAZSDK4RMX.dll  $(NLBUILDROOT)/prod/RMXForSP/depend/
	cp -pf $(MSENFORCECOMMON)/bin/$(BUILDTYPE)_win_$(TARGETENVARCH)/SkyDrmRestHelp.dll  $(NLBUILDROOT)/prod/RMXForSP/depend/
	cp -pf $(MSENFORCECOMMON)/bin/$(BUILDTYPE)_win_$(TARGETENVARCH)/SDKWrapper4RMXLib.dll  $(NLBUILDROOT)/prod/RMXForSP/depend/      

	@if [ ! -d $(NLBUILDROOT)/prod/RMXForSP/NextLabs.RightsManager/Layouts/NextLabs.RightsManager/bin64 ]; then					\
		mkdir $(NLBUILDROOT)/prod/RMXForSP/NextLabs.RightsManager/Layouts/NextLabs.RightsManager/bin64 ;						\
	fi
	cp -pf $(MSENFORCECOMMON)/prod/cepc_bin/bin64/*  $(NLBUILDROOT)/prod/RMXForSP/NextLabs.RightsManager/Layouts/NextLabs.RightsManager/bin64/
	cp -pf $(MSENFORCECOMMON)/bin/$(BUILDTYPE)_win_$(TARGETENVARCH)/SDKWrapper4RMX.dll  $(NLBUILDROOT)/prod/RMXForSP/NextLabs.RightsManager/Layouts/NextLabs.RightsManager/bin64/
	cp -pf $(MSENFORCECOMMON)/bin/$(BUILDTYPE)_win_$(TARGETENVARCH)/SkyDrmRestHelp.dll  $(NLBUILDROOT)/prod/RMXForSP/NextLabs.RightsManager/Layouts/NextLabs.RightsManager/bin64/
	cp -pf $(MSENFORCECOMMON)/bin/$(BUILDTYPE)_win_$(TARGETENVARCH)/QueryCloudAZSDK4RMX.dll  $(NLBUILDROOT)/prod/RMXForSP/NextLabs.RightsManager/Layouts/NextLabs.RightsManager/bin64/

	@if [ ! -d $(NLBUILDROOT)/prod/RMXForSP/NextLabs.RightsManager/Layouts/NextLabs.RightsManager/jars ]; then					\
		mkdir $(NLBUILDROOT)/prod/RMXForSP/NextLabs.RightsManager/Layouts/NextLabs.RightsManager/jars ;						\
	fi
	cp -pf $(MSENFORCECOMMON)/prod/rms_javasdk/RMSERVERSDK-10-0231-201908200009-build/lib/* $(NLBUILDROOT)/prod/RMXForSP/NextLabs.RightsManager/Layouts/NextLabs.RightsManager/jars/

.PHONY: sp2019
sp2019:
	@echo ""
	@echo "Building $(PROJECT_SP2019) ($(BUILDTYPE)) for RMX for SharePoint"
	rm -rf $(BUILD_LOGFILE_SP2019)
	cp -rpf ./Package_SP20162019/* ./Package/
	cp -rpf ./Page_SP20162019/Layouts/NextLabs.RightsManager/* ./Layouts/NextLabs.RightsManager/
	cp -rpf ./RMS_SP20162019/* ./RMS/
	$(MSVSIDE) $(PROJECT_SP2019) /build "$(PROJECT_CONFIG_SP2019)|AnyCPU" /out $(BUILD_LOGFILE_SP2019) ; \
	COMPILE_STATUS=$$? ;									\
	if [ -f $(BUILD_LOGFILE_SP2019) ] ; then				\
		echo "[[DUMP BEGIN - $(BUILD_LOGFILE_SP2019)]]" ;	\
		cat $(BUILD_LOGFILE_SP2019) ;						\
		echo "[[DUMP END - $(BUILD_LOGFILE_SP2019)]]" ;		\
	else													\
		echo "INFO: Cannot find $(BUILD_LOGFILE_SP2019)" ;	\
	fi ;													\
	exit $$COMPILE_STATUS
	@if [ $(OFFICIALCERT) -ne 0 ]; then											\
		echo $(SIGNTOOL_OFFICIAL) $(PROJECT_CONFIG_SP2019)/NextLabs.RightsManager.dll ; \
		$(SIGNTOOL_OFFICIAL) $(PROJECT_CONFIG_SP2019)/NextLabs.RightsManager.dll ; \
	fi
	@if [ ! -d $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2019) ]; then					\
		mkdir -p $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2019) ;						\
	fi
	cp -f $(PROJECT_CONFIG_SP2019)/NextLabs.RightsManager.wsp $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2019)

.PHONY: sp2016
sp2016:
	@echo ""
	@echo "Building $(PROJECT_SP2016) ($(BUILDTYPE)) for RMX for SharePoint"
	rm -rf $(BUILD_LOGFILE_SP2016)
	cp -rpf ./Package_SP20162019/* ./Package/
	cp -rpf ./Page_SP20162019/Layouts/NextLabs.RightsManager/* ./Layouts/NextLabs.RightsManager/
	cp -rpf ./RMS_SP20162019/* ./RMS/
	$(MSVSIDE) $(PROJECT_SP2016) /build "$(PROJECT_CONFIG_SP2016)|AnyCPU" /out $(BUILD_LOGFILE_SP2016) ; \
	COMPILE_STATUS=$$? ;									\
	if [ -f $(BUILD_LOGFILE_SP2016) ] ; then				\
		echo "[[DUMP BEGIN - $(BUILD_LOGFILE_SP2016)]]" ;	\
		cat $(BUILD_LOGFILE_SP2016) ;						\
		echo "[[DUMP END - $(BUILD_LOGFILE_SP2016)]]" ;		\
	else													\
		echo "INFO: Cannot find $(BUILD_LOGFILE_SP2016)" ;	\
	fi ;													\
	exit $$COMPILE_STATUS
	@if [ $(OFFICIALCERT) -ne 0 ]; then											\
		echo $(SIGNTOOL_OFFICIAL) $(PROJECT_CONFIG_SP2016)/NextLabs.RightsManager.dll ; \
		$(SIGNTOOL_OFFICIAL) $(PROJECT_CONFIG_SP2016)/NextLabs.RightsManager.dll ; \
	fi
	@if [ ! -d $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2016) ]; then					\
		mkdir -p $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2016) ;						\
	fi
	cp -f $(PROJECT_CONFIG_SP2016)/NextLabs.RightsManager.wsp $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2016)

.PHONY: sp2013
sp2013:
	@echo ""
	@echo "Building $(PROJECT_SP2013) ($(BUILDTYPE)) for RMX for SharePoint"
	rm -rf $(BUILD_LOGFILE_SP2013)
	cp -rpf ./Package_SP2013/* ./Package/
	cp -rpf ./Page_SP2013/Layouts/NextLabs.RightsManager/* ./Layouts/NextLabs.RightsManager/
	cp -rpf ./RMS_SP2013/* ./RMS/
	$(MSVSIDE) $(PROJECT_SP2013) /build "$(PROJECT_CONFIG_SP2013)|AnyCPU" /out $(BUILD_LOGFILE_SP2013) ; \
	COMPILE_STATUS=$$? ;									\
	if [ -f $(BUILD_LOGFILE_SP2013) ] ; then				\
		echo "[[DUMP BEGIN - $(BUILD_LOGFILE_SP2013)]]" ;	\
		cat $(BUILD_LOGFILE_SP2013) ;						\
		echo "[[DUMP END - $(BUILD_LOGFILE_SP2013)]]" ;		\
	else													\
		echo "INFO: Cannot find $(BUILD_LOGFILE_SP2013)" ;	\
	fi ;													\
	exit $$COMPILE_STATUS
	@if [ $(OFFICIALCERT) -ne 0 ]; then											\
		echo $(SIGNTOOL_OFFICIAL) $(PROJECT_CONFIG_SP2013)/NextLabs.RightsManager.dll ; \
		$(SIGNTOOL_OFFICIAL) $(PROJECT_CONFIG_SP2013)/NextLabs.RightsManager.dll ; \
	fi
	@if [ ! -d $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2013) ]; then					\
		mkdir -p $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2013) ;						\
	fi
	cp -f $(PROJECT_CONFIG_SP2013)/NextLabs.RightsManager.wsp $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2013)

.PHONY: clean
clean:
	rm -rf $(BUILD_LOGFILE_SP2019)
	rm -rf $(BUILD_LOGFILE_SP2016)
	rm -rf release debug sp2019release sp2019debug sp2016release sp2016debug obj* *.suo *.ncb *.pdb *.wsp $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2019)/NextLabs.Deployment.dll $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2019)/NextLabs.Deployment.pdb $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2016)/NextLabs.Deployment.dll $(BUILDOUTPUTDIR)/$(PROJECT_CONFIG_SP2016)/NextLabs.Deployment.pdb
