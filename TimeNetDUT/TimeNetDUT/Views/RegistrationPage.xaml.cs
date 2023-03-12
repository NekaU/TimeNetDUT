﻿using System.Net.Http;
using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http.Headers;
using TimeNetDUT.Utils;

namespace TimeNetDUT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegistrationPage : ContentPage
    {
        ContentPage page;
        string cookie;
        HttpClient httpClient = new HttpClient();
        UserConfig user = UserConfigManager.LoadConfig();
        Xamarin.Forms.Picker picker;
        Xamarin.Forms.Picker pickerCourse;
        Xamarin.Forms.Picker pickerGroup;

        public RegistrationPage ()
		{
            InitializeComponent ();
            page = RegistrationContentPage;
            Button studentBtn = page.FindByName<Button>("studentBtn");
            Button teacherBtn = page.FindByName<Button>("teacherBtn");

            studentBtn.Clicked += StudentRegistration;
            teacherBtn.Clicked += TeacherRegistration;
        }

        private async void StudentRegistration(object sender, EventArgs e)
        {
            page.Content = null;

            var header = new Label
            {
                Text = "Оберіть факультет:",
                TextColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) // Установка размера шрифта
            };

            picker = new Xamarin.Forms.Picker()
            {
                Title = "Факультет",
                AutomationId = "FaculityPicker",
                IsVisible = false
            };

            pickerCourse = new Xamarin.Forms.Picker()
            {
                Title = "Курс",
                AutomationId = "CoursePicker",
                IsVisible = false
            };

            pickerGroup = new Xamarin.Forms.Picker()
            {
                Title = "Група",
                AutomationId = "GroupPicker",
                IsVisible = false
            };

            var activityIndicator = new ActivityIndicator
            {
                IsRunning = true
            };

            var stackLayout = new StackLayout
            {
                Children =
                {
                    header,
                    picker,
                    pickerCourse,
                    pickerGroup
                }
            };

            page.Content = activityIndicator;

            try
            {
                var result = await GetFacultetAsync();
                var faculities = result.Faculties;
                cookie = result.Cookie;

                picker.ItemsSource = faculities;
                picker.ItemDisplayBinding = new Binding("Text");
                picker.IsVisible = true;
                picker.SelectedIndexChanged += PickerFacultiesChangedIndex;

                page.Content = stackLayout;
            }
            catch (Exception ex)
            {
                // TODO: Обработка исключения
            }
        }



        private void TeacherRegistration(object sender, EventArgs e)
        {
            page.Content = null;
        }

        
        private async void PickerFacultiesChangedIndex(object sender, EventArgs e)
        {
            if (picker.ItemsSource == null || picker.SelectedIndex < 0)
            {
                return;
            }

            Option selectedFaculty = (Option)picker.SelectedItem;
            user.FacultyId = Convert.ToInt32(selectedFaculty.Value);

            try
            {
                List<Option> courses = await GetCoursesAsync();

                pickerCourse.ItemsSource = courses;
                pickerCourse.ItemDisplayBinding = new Binding("Text");
                pickerCourse.SelectedIndexChanged += PickerCoursesChangedIndex;

                pickerCourse.IsVisible = true;
                pickerGroup.IsVisible = false;
            }
            catch (Exception ex)
            {
                // TODO: Обработка исключения
            }
        }

        private void PickerCoursesChangedIndex(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private class FacultiesAndCookie
        {
            public List<Option> Faculties { get; set; }
            public string Cookie { get; set; }
        }

        private async Task<FacultiesAndCookie> GetFacultetAsync()
        {
            var response = await httpClient.GetAsync("https://e-rozklad.dut.edu.ua/time-table/group");
            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var select = doc.GetElementbyId("timetableform-facultyid");
            var options = select.Descendants("option").Select(o => new Option
            {
                Value = o.GetAttributeValue("value", ""),
                Text = o.InnerText.Trim()
            }).ToList();

            List<Option> faculity = new List<Option>();
            foreach (var option in options)
            {
                if (!string.IsNullOrEmpty(option.Text) && !string.IsNullOrEmpty(option.Value))
                    faculity.Add(option);
            }

            var input = doc.DocumentNode.SelectSingleNode("//input[@name='_csrf-frontend']");
            var value = input?.GetAttributeValue("value", "");
            string cookie = value;

            return new FacultiesAndCookie
            {
                Faculties = faculity,
                Cookie = cookie
            };
        }

        private async Task<List<Option>> GetCoursesAsync()
        {
            // Запрос курса
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            httpClient.DefaultRequestHeaders.Add("Host", "e-rozklad.dut.edu.ua");
            httpClient.DefaultRequestHeaders.Add("X-CSRF-Token", cookie);

            // создание тела запроса
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("TimeTableForm[facultyId]", user.FacultyId.ToString())
            });

            // отправка POST-запроса и получение результата
            var responses = await httpClient.PostAsync("https://e-rozklad.dut.edu.ua/time-table/group", content);
            var result = await responses.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(result);

            var select = doc.GetElementbyId("timetableform-course");
            var options = select.Descendants("option")
                .Select(o => new Option
                {
                    Value = o.GetAttributeValue("value", ""),
                    Text = o.InnerText.Trim()
                })
                .Where(o => !string.IsNullOrEmpty(o.Text) && !string.IsNullOrEmpty(o.Value))
                .ToList();

            return options;
        }



        private class Option
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }


        private async void GetSchedule()
        {
            // Запрос факультета
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://e-rozklad.dut.edu.ua/time-table/group");
            var html = await response.Content.ReadAsStringAsync();


            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var select = doc.GetElementbyId("timetableform-facultyid");
            var options = select.Descendants("option").Select(o => new {
                Value = o.GetAttributeValue("value", ""),
                Text = o.InnerText.Trim()
            }).ToList();
            var input = doc.DocumentNode.SelectSingleNode("//input[@name='_csrf-frontend']");
            var value = input?.GetAttributeValue("value", "");
            var option = select.Descendants("option").FirstOrDefault(o => o.GetAttributeValue("value", "") == "1");


            if (option != null)
            {
                option.SetAttributeValue("selected", "selected");
                var selectedValue = option.GetAttributeValue("value", "");
            }



            using (var httpClient3 = new HttpClient())
            {
                var httpClient2 = new HttpClient();
                // Запрос курса
                httpClient2.DefaultRequestHeaders.Accept.Clear();
                httpClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                httpClient2.DefaultRequestHeaders.Add("Cookie", "SLG_G_WPT_TO=ru; advanced-frontend=j1u5ksm3d71ckf3kr9rg3v1vf9; SLG_GWPT_Show_Hide_tmp=1; SLG_wptGlobTipTmp=1; _csrf-frontend=8e07c93c476602b815148c1b6f3dfb74a6959a75aee3d54e31333dcabaf60197a%3A2%3A%7Bi%3A0%3Bs%3A14%3A%22_csrf-frontend%22%3Bi%3A1%3Bs%3A32%3A%22R4E9O7HSZJIMJb2gpHDbMaLMbLgrMNvd%22%3B%7D");
                httpClient2.DefaultRequestHeaders.Add("Host", "e-rozklad.dut.edu.ua");
                httpClient2.DefaultRequestHeaders.Add("X-CSRF-Token", "78YCe7vrEPC4MxcltkdAdd56oEWPI4P4yATedYCVDte98kdC9NxYo-J5Xmj8JXISrjLkJ8JCz7WqSLkHzdt4sw==");

                // создание тела запроса
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("TimeTableForm[facultyId]", "3")
                });

                // отправка POST-запроса и получение результата
                var responses = await httpClient2.PostAsync("https://e-rozklad.dut.edu.ua/time-table/group", content);
                var result = await responses.Content.ReadAsStringAsync();

                doc = new HtmlDocument();
                doc.LoadHtml(result);

                select = doc.GetElementbyId("timetableform-course");
                options = select.Descendants("option").Select(o => new {
                    Value = o.GetAttributeValue("value", ""),
                    Text = o.InnerText.Trim()
                }).ToList();
                value = input?.GetAttributeValue("value", "");
                option = select.Descendants("option").FirstOrDefault(o => o.GetAttributeValue("value", "") == "1");

                if (option != null)
                {
                    option.SetAttributeValue("selected", "selected");
                    var selectedValue = option.GetAttributeValue("value", "");
                }


                // Запрос группы
                httpClient2 = new HttpClient();
                httpClient2.DefaultRequestHeaders.Accept.Clear();
                httpClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                httpClient2.DefaultRequestHeaders.Add("Cookie", "SLG_G_WPT_TO=ru; advanced-frontend=j1u5ksm3d71ckf3kr9rg3v1vf9; SLG_GWPT_Show_Hide_tmp=1; SLG_wptGlobTipTmp=1; _csrf-frontend=8e07c93c476602b815148c1b6f3dfb74a6959a75aee3d54e31333dcabaf60197a%3A2%3A%7Bi%3A0%3Bs%3A14%3A%22_csrf-frontend%22%3Bi%3A1%3Bs%3A32%3A%22R4E9O7HSZJIMJb2gpHDbMaLMbLgrMNvd%22%3B%7D");
                httpClient2.DefaultRequestHeaders.Add("Host", "e-rozklad.dut.edu.ua");
                httpClient2.DefaultRequestHeaders.Add("X-CSRF-Token", "78YCe7vrEPC4MxcltkdAdd56oEWPI4P4yATedYCVDte98kdC9NxYo-J5Xmj8JXISrjLkJ8JCz7WqSLkHzdt4sw==");

                // создание тела запроса
                content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("TimeTableForm[facultyId]", "3"),
                    new KeyValuePair<string, string>("TimeTableForm[course]", "1")
                });

                // отправка POST-запроса и получение результата
                responses = await httpClient2.PostAsync("https://e-rozklad.dut.edu.ua/time-table/group", content);
                result = await responses.Content.ReadAsStringAsync();

                doc = new HtmlDocument();
                doc.LoadHtml(result);

                select = doc.GetElementbyId("timetableform-groupid");
                options = select.Descendants("option").Select(o => new {
                    Value = o.GetAttributeValue("value", ""),
                    Text = o.InnerText.Trim()
                }).ToList();
                value = input?.GetAttributeValue("value", "");
                option = select.Descendants("option").FirstOrDefault(o => o.GetAttributeValue("value", "") == "1");

                if (option != null)
                {
                    option.SetAttributeValue("selected", "selected");
                    var selectedValue = option.GetAttributeValue("value", "");
                }

                httpClient2 = new HttpClient();
                httpClient2.DefaultRequestHeaders.Accept.Clear();
                httpClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                httpClient2.DefaultRequestHeaders.Add("Cookie", "SLG_G_WPT_TO=ru; advanced-frontend=j1u5ksm3d71ckf3kr9rg3v1vf9; SLG_GWPT_Show_Hide_tmp=1; SLG_wptGlobTipTmp=1; _csrf-frontend=8e07c93c476602b815148c1b6f3dfb74a6959a75aee3d54e31333dcabaf60197a%3A2%3A%7Bi%3A0%3Bs%3A14%3A%22_csrf-frontend%22%3Bi%3A1%3Bs%3A32%3A%22R4E9O7HSZJIMJb2gpHDbMaLMbLgrMNvd%22%3B%7D");
                httpClient2.DefaultRequestHeaders.Add("Host", "e-rozklad.dut.edu.ua");
                httpClient2.DefaultRequestHeaders.Add("X-CSRF-Token", "78YCe7vrEPC4MxcltkdAdd56oEWPI4P4yATedYCVDte98kdC9NxYo-J5Xmj8JXISrjLkJ8JCz7WqSLkHzdt4sw==");

                // создание тела запроса
                content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("TimeTableForm[facultyId]", "3"),
                    new KeyValuePair<string, string>("TimeTableForm[course]", "1"),
                    new KeyValuePair<string, string>("TimeTableForm[groupId]", "1568")
                });

                // отправка POST-запроса и получение результата
                responses = await httpClient2.PostAsync("https://e-rozklad.dut.edu.ua/time-table/group", content);
                result = await responses.Content.ReadAsStringAsync();

                doc = new HtmlDocument();
                doc.LoadHtml(result);
            }



        }
    }
}