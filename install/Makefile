include $(NLBUILDROOT)/build/Makefile.ver

VERSION_STR=$(VERSION_MAJOR_RMX4SP).$(VERSION_MINOR_RMX4SP)

ASSEMBLY_DIR_SP2019=assembly2019
PACKAGE_FILE_SP2019=NextlabsRightsManagerForSharePoint2019-$(VERSION_STR)-$(VERSION_BUILD_SHORT)-$(shell date +"%Y%m%d%H%M").zip

ASSEMBLY_DIR_SP2016=assembly2016
PACKAGE_FILE_SP2016=NextlabsRightsManagerForSharePoint2016-$(VERSION_STR)-$(VERSION_BUILD_SHORT)-$(shell date +"%Y%m%d%H%M").zip

ASSEMBLY_DIR_SP2013=assembly2013
PACKAGE_FILE_SP2013=NextlabsRightsManagerForSharePoint2013-$(VERSION_STR)-$(VERSION_BUILD_SHORT)-$(shell date +"%Y%m%d%H%M").zip

DEST_INSTALL_DIR=$(NLBUILDROOT)/bin/install
.PHONY: all
all: installerSP2019 installerSP2016 installerSP2013

PHONY: installerSP2019
installerSP2019:
	@echo "INFO: Make installer for SP2019"
	@if [ ! -d $(ASSEMBLY_DIR_SP2019) ]; then					\
		mkdir $(ASSEMBLY_DIR_SP2019) ;						\
	fi
	cp -pf $(NLBUILDROOT)/bin/release_dotnet/sp2019release/*  $(ASSEMBLY_DIR_SP2019)/
	@echo "INFO: Zip assembly"
	cd $(ASSEMBLY_DIR_SP2019); zip ../$(PACKAGE_FILE_SP2019) *
	cd ..
	@if [ ! -d $(DEST_INSTALL_DIR) ]; then					\
		mkdir $(DEST_INSTALL_DIR) ;						\
	fi
	cp -f $(PACKAGE_FILE_SP2019) $(DEST_INSTALL_DIR)/



PHONY: installerSP2016
installerSP2016:
	@echo "INFO: Make installer for SP2016"
	@if [ ! -d $(ASSEMBLY_DIR_SP2016) ]; then					\
		mkdir $(ASSEMBLY_DIR_SP2016) ;						\
	fi
	cp -pf $(NLBUILDROOT)/bin/release_dotnet/sp2016release/*  $(ASSEMBLY_DIR_SP2016)/
	@echo "INFO: Zip assembly"
	cd $(ASSEMBLY_DIR_SP2016); zip ../$(PACKAGE_FILE_SP2016) *
	cd ..
	@if [ ! -d $(DEST_INSTALL_DIR) ]; then					\
		mkdir $(DEST_INSTALL_DIR) ;						\
	fi
	cp -f $(PACKAGE_FILE_SP2016) $(DEST_INSTALL_DIR)/

PHONY: installerSP2013
installerSP2013:
	@echo "INFO: Make installer for SP2013"
	@if [ ! -d $(ASSEMBLY_DIR_SP2013) ]; then					\
		mkdir $(ASSEMBLY_DIR_SP2013) ;						\
	fi
	cp -pf $(NLBUILDROOT)/bin/release_dotnet/sp2013release/*  $(ASSEMBLY_DIR_SP2013)/
	@echo "INFO: Zip assembly"
	cd $(ASSEMBLY_DIR_SP2013); zip ../$(PACKAGE_FILE_SP2013) *
	cd ..
	@if [ ! -d $(DEST_INSTALL_DIR) ]; then					\
		mkdir $(DEST_INSTALL_DIR) ;						\
	fi
	cp -f $(PACKAGE_FILE_SP2013) $(DEST_INSTALL_DIR)/