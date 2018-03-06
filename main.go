package main

import (
	"bufio"
	"errors"
	"github.com/comwrg/mop-go"
	_"github.com/mattn/go-sqlite3"
	"io/ioutil"
	"log"
	"net/http"
	"os"
	"runtime"
	"strconv"
	"strings"
	"time"
)

var mp mop.Protocol
var sqlite mop.Sqlite
var Info    = log.New(os.Stdout, "[INFO   ] ", log.Ldate | log.Ltime)
var Warning = log.New(os.Stdout, "[WARNING] ", log.Ldate | log.Ltime)
var Error   = log.New(os.Stderr, "[ERROR  ] ", log.Ldate | log.Ltime)

func getConf() (mobile string, pwd string, err error) {
	bin, err := ioutil.ReadFile("user.txt")
	if err != nil {
		return
	}
	str := string(bin)
	arr := strings.Split(str, " ")
	if len(arr) != 2 {
		err = errors.New("conf error, can't split")
		return
	}
	mobile = arr[0]
	pwd = arr[1]
	return
}

func getVc(mobile string, pwd string) (vc string, err error) {
	url := "http://144.48.7.239:1228/getvc?mobile={mobile}&pwd={pwd}"
	url = strings.Replace(url, "{mobile}", mobile, 1)
	url = strings.Replace(url, "{pwd}", pwd, 1)
	client := http.Client{}
	resp, err := client.Get(url)
	if err != nil {
		return
	}
	bin, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return
	}
	bin, err = mop.GbkToUtf8(bin)
	if err != nil {
		return
	}
	vc = string(bin)
	if vc == "error" {
		err = errors.New("error")
		return
	}
	return
}

func exit() {
	Info.Println("The program will exit one minute later.")
	time.Sleep(time.Minute)
	os.Exit(0)
}

func check(msg string, err error) {
	if err != nil {
		Error.Println(msg, "Error:", err.Error())
		exit()
	}
}

func zfill(str string, width int) (string){
	for i := 0; i < width - len(str); i++ {
		str = "0" + str
	}
	return str[:width]
}

func handle(mobile string) {
	var err error

	userInfo := mop.JSONuserInfo{}
	err = mp.Query(mobile, &userInfo)
	check("query userInfo err.", err)
	if !userInfo.Success {
		return
	}
	err = sqlite.UpdateUserInfo(mobile, mop.FilterUserInfo(&userInfo))
	check("sqlite update userInfo err.", err)

	businessInfo := mop.JSONbusinessInfo{}
	err = mp.Query(mobile, &businessInfo)
	check("query businessInfo err.", err)
	filterBusinessInfo, err := mop.FilterBusinessInfo(&businessInfo)
	check("filterBusinessInfo err.", err)
	err = sqlite.UpdateBusinessInfo(mobile, filterBusinessInfo)
	check("updataBussinessInfo err.", err)

	consumeInfo := mop.JSONconsumeInfo{}
	err = mp.Query(mobile, &consumeInfo)
	check("query consumeInfo err.", err)
	filterConsumeInfo, err := mop.FilterConsumeInfo(&consumeInfo)
	check("filterConsumeInfo err.", err)
	err = sqlite.UpdateConsumeInfo(mobile, filterConsumeInfo)
	check("updataConsumeInfo err.", err)

	userBaseInfo := mop.JSONuserBaseInfo{}
	err = mp.Query(mobile, &userBaseInfo)
	check("query userBaseInfo err.", err)
	filterUserBaseInfo, err := mop.FilterUserBaseInfo(&userBaseInfo)
	check("filterUserBaseInfo err.", err)
	err = sqlite.UpdateUserBaseInfo(mobile, filterUserBaseInfo)
	check("updateUserBaseInfo err.", err)

}

func main() {
	numGoroutine := runtime.NumGoroutine()
	
	err := sqlite.Init()
	check("sqlite init failed.", err)

	mobile, pwd, err := getConf()
	Info.Println("Read Conf:", mobile, pwd)
	vc, err := getVc(mobile, pwd)
	check("Login Failed. ", err)
	Info.Println("vc:", vc)
	err = mp.Init(vc)
	check("Init Failed. ", err)

	// read line, reference: https://gist.github.com/kendellfab/7417164
	inFile, err := os.Open("number.txt")
	defer inFile.Close()
	check("Read txt failed.", err)
	scanner := bufio.NewScanner(inFile)
	scanner.Split(bufio.ScanLines)

	for scanner.Scan() {
		ln := scanner.Text()
		ln = strings.TrimSpace(ln)
		if ln == "" {
			continue
		}
		tailWidth := 11 - len(ln)

		tailTop := 1
		for i := 0; i < tailWidth; i++ {
			tailTop *= 10
		}

		for i := 0; i < tailTop; i++ {
			tail := zfill(strconv.Itoa(i), tailWidth)
			mobile := ln + tail
			sqlite.Insert(mobile)
			go handle(mobile)
		}
	}

	for numGoroutine < runtime.NumGoroutine() {
		time.Sleep(time.Minute)
	}
	Info.Println("Finish!")
	exit()
}