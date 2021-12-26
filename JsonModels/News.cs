using System.Text.Json.Serialization;

namespace ValorantManager.JsonModels
{
    public class News
    {
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
        public class Dimension
        {
            [JsonPropertyName("height")]
            public int Height { get; set; }

            [JsonPropertyName("width")]
            public int Width { get; set; }
        }

        public class Banner
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("dimension")]
            public Dimension Dimension { get; set; }

            [JsonPropertyName("content_type")]
            public string ContentType { get; set; }

            [JsonPropertyName("file_size")]
            public string FileSize { get; set; }

            [JsonPropertyName("filename")]
            public string Filename { get; set; }
        }

        public class Url
        {
            [JsonPropertyName("url")]
            public string url { get; set; }
        }

        public class Node
        {
            [JsonPropertyName("banner")]
            public Banner Banner { get; set; }

            [JsonPropertyName("article_type")]
            public string ArticleType { get; set; }

            [JsonPropertyName("date")]
            public DateTime Date { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("uid")]
            public string Uid { get; set; }

            [JsonPropertyName("external_link")]
            public string ExternalLink { get; set; }

            [JsonPropertyName("url")]
            public Url Url { get; set; }
        }

        public class AllContentstackArticles
        {
            [JsonPropertyName("nodes")]
            public List<Node> Nodes { get; set; }
        }

        public class Reference
        {
            [JsonPropertyName("banner")]
            public Banner Banner { get; set; }

            [JsonPropertyName("article_type")]
            public string ArticleType { get; set; }

            [JsonPropertyName("date")]
            public DateTime Date { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("uid")]
            public string Uid { get; set; }

            [JsonPropertyName("external_link")]
            public string ExternalLink { get; set; }

            [JsonPropertyName("url")]
            public Url Url { get; set; }
        }

        public class FeaturedNews
        {
            [JsonPropertyName("reference")]
            public List<Reference> Reference { get; set; }
        }

        public class ContentstackNews
        {
            [JsonPropertyName("featured_news")]
            public FeaturedNews FeaturedNews { get; set; }
        }

        public class Data
        {
            [JsonPropertyName("allContentstackArticles")]
            public AllContentstackArticles AllContentstackArticles { get; set; }

            [JsonPropertyName("contentstackNews")]
            public ContentstackNews ContentstackNews { get; set; }
        }

        public class Image
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }
        }

        public class Opengraph
        {
            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("image")]
            public Image Image { get; set; }
        }

        public class Twitter
        {
            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("image")]
            public Image Image { get; set; }

            [JsonPropertyName("site")]
            public string Site { get; set; }
        }

        public class OgData
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("opengraph")]
            public Opengraph Opengraph { get; set; }

            [JsonPropertyName("twitter")]
            public Twitter Twitter { get; set; }
        }

        public class Page
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }
        }

        public class Head
        {
            [JsonPropertyName("page")]
            public Page Page { get; set; }
        }

        public class SectionNavigation
        {
            [JsonPropertyName("hero")]
            public string Hero { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonPropertyName("overview")]
            public string Overview { get; set; }

            [JsonPropertyName("social")]
            public string Social { get; set; }

            [JsonPropertyName("faqs")]
            public string Faqs { get; set; }
        }

        public class Mobile
        {
            [JsonPropertyName("label")]
            public string Label { get; set; }

            [JsonPropertyName("href")]
            public string Href { get; set; }
        }

        public class Desktop
        {
            [JsonPropertyName("label")]
            public string Label { get; set; }

            [JsonPropertyName("href")]
            public string Href { get; set; }
        }

        public class Cta
        {
            [JsonPropertyName("mobile")]
            public Mobile Mobile { get; set; }

            [JsonPropertyName("desktop")]
            public Desktop Desktop { get; set; }

            [JsonPropertyName("label")]
            public string Label { get; set; }

            [JsonPropertyName("link")]
            public string Link { get; set; }

            [JsonPropertyName("text")]
            public string Text { get; set; }

            [JsonPropertyName("href")]
            public string Href { get; set; }
        }

        public class Intro
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("subtitle")]
            public string Subtitle { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("cta")]
            public Cta Cta { get; set; }
        }

        public class Enter
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("cta")]
            public string Cta { get; set; }
        }

        public class Email
        {
            [JsonPropertyName("intro")]
            public Intro Intro { get; set; }

            [JsonPropertyName("enter")]
            public Enter Enter { get; set; }

            [JsonPropertyName("confirmation")]
            public string Confirmation { get; set; }

            [JsonPropertyName("error")]
            public string Error { get; set; }
        }

        public class Overview
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("subtitle")]
            public string Subtitle { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("description2")]
            public string Description2 { get; set; }

            [JsonPropertyName("description3")]
            public string Description3 { get; set; }

            [JsonPropertyName("disclaimer")]
            public string Disclaimer { get; set; }

            [JsonPropertyName("tagline")]
            public List<string> Tagline { get; set; }

            [JsonPropertyName("gameplayVideo")]
            public string GameplayVideo { get; set; }

            [JsonPropertyName("gameplayVideoTag")]
            public string GameplayVideoTag { get; set; }

            [JsonPropertyName("cta")]
            public Cta Cta { get; set; }
        }

        public class SeasonUpdates
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("subtitle")]
            public string Subtitle { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("cta")]
            public Cta Cta { get; set; }
        }

        public class Maps
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("subtitle")]
            public string Subtitle { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("tagline")]
            public List<string> Tagline { get; set; }

            [JsonPropertyName("cta")]
            public Cta Cta { get; set; }
        }

        public class Social
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("subtitle")]
            public List<string> Subtitle { get; set; }

            [JsonPropertyName("tagline")]
            public List<string> Tagline { get; set; }

            [JsonPropertyName("findUs")]
            public string FindUs { get; set; }
        }

        public class NoPageFound
        {
            [JsonPropertyName("title")]
            public List<string> Title { get; set; }

            [JsonPropertyName("subtitle")]
            public string Subtitle { get; set; }

            [JsonPropertyName("goToHomepageCopy")]
            public string GoToHomepageCopy { get; set; }
        }

        public class Faqs
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }
        }

        //public class News
        //{
        //    [JsonPropertyName("featuredNews")]
        //    public string FeaturedNews { get; set; }

        //    [JsonPropertyName("previousNews")]
        //    public string PreviousNews { get; set; }

        //    [JsonPropertyName("backToList")]
        //    public string BackToList { get; set; }

        //    [JsonPropertyName("loginAlchemerCopy")]
        //    public string LoginAlchemerCopy { get; set; }

        //    [JsonPropertyName("seeArticle")]
        //    public string SeeArticle { get; set; }

        //    [JsonPropertyName("tagline")]
        //    public List<string> Tagline { get; set; }
        //}

        public class Download
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("cta")]
            public Cta Cta { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("rightHere")]
            public string RightHere { get; set; }

            [JsonPropertyName("go")]
            public string Go { get; set; }
        }

        public class General
        {
            [JsonPropertyName("scrollDown")]
            public List<string> ScrollDown { get; set; }

            [JsonPropertyName("noElements")]
            public string NoElements { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("viewGallery")]
            public string ViewGallery { get; set; }
        }

        public class Join
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }
        }

        public class SignUp
        {
            [JsonPropertyName("title")]
            public object Title { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("cta")]
            public List<Cta> Cta { get; set; }
        }

        public class Headline
        {
            [JsonPropertyName("pendingCopy")]
            public string PendingCopy { get; set; }

            [JsonPropertyName("grantedCopy")]
            public string GrantedCopy { get; set; }
        }

        public class SubCopy
        {
            [JsonPropertyName("pendingSubCopy")]
            public string PendingSubCopy { get; set; }

            [JsonPropertyName("grantedSubCopy")]
            public string GrantedSubCopy { get; set; }
        }

        public class Step
        {
            [JsonPropertyName("stepHeadline")]
            public string StepHeadline { get; set; }

            [JsonPropertyName("cta")]
            public Cta Cta { get; set; }

            [JsonPropertyName("stepSubCopy")]
            public List<string> StepSubCopy { get; set; }
        }

        public class StatusCheck
        {
            [JsonPropertyName("backgroundText")]
            public string BackgroundText { get; set; }

            [JsonPropertyName("headline")]
            public Headline Headline { get; set; }

            [JsonPropertyName("subCopy")]
            public SubCopy SubCopy { get; set; }

            [JsonPropertyName("tagline")]
            public List<string> Tagline { get; set; }

            [JsonPropertyName("steps")]
            public List<Step> Steps { get; set; }
        }

        public class MediaGalleryOverview
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("imageUrl")]
            public string ImageUrl { get; set; }
        }

        public class AgentSpecialAbilities
        {
            [JsonPropertyName("headline")]
            public string Headline { get; set; }

            [JsonPropertyName("tagline")]
            public List<string> Tagline { get; set; }
        }

        public class PaperClips
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("tagline")]
            public List<string> Tagline { get; set; }

            [JsonPropertyName("backgroundText")]
            public List<string> BackgroundText { get; set; }
        }

        public class Weapons
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("all")]
            public string All { get; set; }

            [JsonPropertyName("tagline")]
            public List<string> Tagline { get; set; }

            [JsonPropertyName("taglineRed")]
            public List<string> TaglineRed { get; set; }
        }

        public class WeaponsDetail
        {
            [JsonPropertyName("tagline")]
            public List<string> Tagline { get; set; }
        }

        public class MediaModal
        {
            [JsonPropertyName("downloadTitle")]
            public string DownloadTitle { get; set; }

            [JsonPropertyName("fileSize")]
            public string FileSize { get; set; }

            [JsonPropertyName("assetType")]
            public string AssetType { get; set; }
        }

        public class Media
        {
            [JsonPropertyName("categoryAll")]
            public string CategoryAll { get; set; }

            [JsonPropertyName("dropdownAll")]
            public string DropdownAll { get; set; }

            [JsonPropertyName("dropdownSelect")]
            public string DropdownSelect { get; set; }
        }

        public class Agents
        {
            [JsonPropertyName("roleLabel")]
            public string RoleLabel { get; set; }

            [JsonPropertyName("biographyLabel")]
            public string BiographyLabel { get; set; }

            [JsonPropertyName("introTitle")]
            public string IntroTitle { get; set; }

            [JsonPropertyName("introDetail")]
            public string IntroDetail { get; set; }

            [JsonPropertyName("dropdownPlaceholder")]
            public string DropdownPlaceholder { get; set; }
        }

        public class Specs
        {
            [JsonPropertyName("heroDescription")]
            public string HeroDescription { get; set; }

            [JsonPropertyName("requirementsBackgroundText")]
            public List<string> RequirementsBackgroundText { get; set; }
        }

        public class Labels
        {
            [JsonPropertyName("day")]
            public string Day { get; set; }

            [JsonPropertyName("month")]
            public string Month { get; set; }

            [JsonPropertyName("year")]
            public string Year { get; set; }

            [JsonPropertyName("continue")]
            public string Continue { get; set; }
        }

        public class VerifyAge
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("notAllowed")]
            public string NotAllowed { get; set; }

            [JsonPropertyName("storageDisclaimer")]
            public string StorageDisclaimer { get; set; }

            [JsonPropertyName("labels")]
            public Labels Labels { get; set; }
        }

        public class Input
        {
            [JsonPropertyName("label")]
            public string Label { get; set; }

            [JsonPropertyName("placeholder")]
            public string Placeholder { get; set; }

            [JsonPropertyName("errorMessage")]
            public string ErrorMessage { get; set; }
        }

        public class TopCopy
        {
            [JsonPropertyName("radiant")]
            public string Radiant { get; set; }

            [JsonPropertyName("immortal")]
            public string Immortal { get; set; }

            [JsonPropertyName("immortal2")]
            public string Immortal2 { get; set; }

            [JsonPropertyName("immortal3")]
            public string Immortal3 { get; set; }
        }

        public class TopCopyDropdown
        {
            [JsonPropertyName("radiant")]
            public string Radiant { get; set; }

            [JsonPropertyName("immortal")]
            public string Immortal { get; set; }

            [JsonPropertyName("immortal2")]
            public string Immortal2 { get; set; }

            [JsonPropertyName("immortal3")]
            public string Immortal3 { get; set; }
        }

        public class Leaderboards
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("input")]
            public Input Input { get; set; }

            [JsonPropertyName("topCopy")]
            public TopCopy TopCopy { get; set; }

            [JsonPropertyName("topCopyDropdown")]
            public TopCopyDropdown TopCopyDropdown { get; set; }

            [JsonPropertyName("leaderboardCopy")]
            public string LeaderboardCopy { get; set; }

            [JsonPropertyName("actCopy")]
            public string ActCopy { get; set; }

            [JsonPropertyName("rankCopy")]
            public string RankCopy { get; set; }

            [JsonPropertyName("ratingCopy")]
            public string RatingCopy { get; set; }

            [JsonPropertyName("activeCopy")]
            public string ActiveCopy { get; set; }

            [JsonPropertyName("inactiveCopy")]
            public string InactiveCopy { get; set; }

            [JsonPropertyName("gamesWonCopy")]
            public string GamesWonCopy { get; set; }

            [JsonPropertyName("pageCopy")]
            public string PageCopy { get; set; }

            [JsonPropertyName("loginCopy")]
            public string LoginCopy { get; set; }

            [JsonPropertyName("minimumThresholdCopy")]
            public string MinimumThresholdCopy { get; set; }

            [JsonPropertyName("rankedRatingCopy")]
            public string RankedRatingCopy { get; set; }
        }

        public class Localization
        {
            [JsonPropertyName("head")]
            public Head Head { get; set; }

            [JsonPropertyName("sectionNavigation")]
            public SectionNavigation SectionNavigation { get; set; }

            [JsonPropertyName("intro")]
            public Intro Intro { get; set; }

            [JsonPropertyName("email")]
            public Email Email { get; set; }

            [JsonPropertyName("overview")]
            public List<Overview> Overview { get; set; }

            [JsonPropertyName("seasonUpdates")]
            public SeasonUpdates SeasonUpdates { get; set; }

            [JsonPropertyName("maps")]
            public Maps Maps { get; set; }

            [JsonPropertyName("social")]
            public Social Social { get; set; }

            [JsonPropertyName("noPageFound")]
            public NoPageFound NoPageFound { get; set; }

            [JsonPropertyName("faqs")]
            public Faqs Faqs { get; set; }

            //[JsonPropertyName("news")]
            //public News News { get; set; }

            [JsonPropertyName("download")]
            public Download Download { get; set; }

            [JsonPropertyName("general")]
            public General General { get; set; }

            [JsonPropertyName("join")]
            public Join Join { get; set; }

            [JsonPropertyName("signUp")]
            public List<SignUp> SignUp { get; set; }

            [JsonPropertyName("statusCheck")]
            public StatusCheck StatusCheck { get; set; }

            [JsonPropertyName("mediaGalleryOverview")]
            public MediaGalleryOverview MediaGalleryOverview { get; set; }

            [JsonPropertyName("agentSpecialAbilities")]
            public AgentSpecialAbilities AgentSpecialAbilities { get; set; }

            [JsonPropertyName("paperClips")]
            public PaperClips PaperClips { get; set; }

            [JsonPropertyName("weapons")]
            public Weapons Weapons { get; set; }

            [JsonPropertyName("weaponsDetail")]
            public WeaponsDetail WeaponsDetail { get; set; }

            [JsonPropertyName("mediaModal")]
            public MediaModal MediaModal { get; set; }

            [JsonPropertyName("media")]
            public Media Media { get; set; }

            [JsonPropertyName("agents")]
            public Agents Agents { get; set; }

            [JsonPropertyName("specs")]
            public Specs Specs { get; set; }

            [JsonPropertyName("verifyAge")]
            public VerifyAge VerifyAge { get; set; }

            [JsonPropertyName("leaderboards")]
            public Leaderboards Leaderboards { get; set; }
        }

        public class LocalizedRoutes
        {
            [JsonPropertyName("agents")]
            public string Agents { get; set; }

            [JsonPropertyName("news")]
            public string News { get; set; }

            [JsonPropertyName("maps")]
            public string Maps { get; set; }

            [JsonPropertyName("media")]
            public string Media { get; set; }

            [JsonPropertyName("arsenal")]
            public string Arsenal { get; set; }

            [JsonPropertyName("specs")]
            public string Specs { get; set; }

            [JsonPropertyName("download")]
            public string Download { get; set; }

            [JsonPropertyName("pbe")]
            public string Pbe { get; set; }

            [JsonPropertyName("pbe-download")]
            public string PbeDownload { get; set; }
        }

        public class PageContext
        {
            [JsonPropertyName("ogData")]
            public OgData OgData { get; set; }

            [JsonPropertyName("environment")]
            public string Environment { get; set; }

            [JsonPropertyName("localization")]
            public Localization Localization { get; set; }

            [JsonPropertyName("locale")]
            public string Locale { get; set; }

            [JsonPropertyName("originalPath")]
            public string OriginalPath { get; set; }

            [JsonPropertyName("bcp47locale")]
            public string Bcp47locale { get; set; }

            [JsonPropertyName("localizedRoutes")]
            public LocalizedRoutes LocalizedRoutes { get; set; }
        }

        public class Result
        {
            [JsonPropertyName("data")]
            public Data Data { get; set; }
        }

        public class Root
        {

            [JsonPropertyName("result")]
            public Result Result { get; set; }
        }


    }
}
